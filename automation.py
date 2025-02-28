import os
from github import Github, GithubException
from crewai import Agent

# Verifica se a variável de ambiente está definida
token = os.environ.get('GITHUB_TOKEN')
if not token:
    raise ValueError('GITHUB_TOKEN environment variable is not set')

g = Github(token)
g_user = g.get_user()

# Define o caminho e o conteúdo do workflow
workflow_path = ".github/workflows/update-docs.yml"
workflow_content = """name: Atualizar Documentação
on:
  push:
    branches:
      - main
  schedule:
    - cron: '0 0 * * *'
jobs:
  update_documentation:
    runs-on: ubuntu-latest
    steps:
      - name: Checkout code
        uses: actions/checkout@v2
      - name: Set up Python
        uses: actions/setup-python@v2
        with:
          python-version: '3.11'
      - name: Instalar uv
        run: pip install uv

      - name: Instalar dependências com uv
        run: uv pip install -e .

      - name: Rodar o script principal
        run: uv venv exec python automation.py

      - name: Install github
        run: pip install github
      - name: Install crewai
        run: pip install crewai
      - name: Update documentation
        run: python automation.py
      - name: Commit changes
        run: |
          git config --global user.email "github-actions@github.com"
          git config --global user.name "GitHub Actions"
          git add .
          git commit -m "Update documentation"
          git push origin main
"""

# Define o caminho e o conteúdo do script de automação
script_path = "automation.py"
script_content = """#!/usr/bin/env python3
import os
from crewai import Agent
from crewai.tools import PythonREPLTool
from langchain_groq import ChatGroq

# Defina a chave da API da Groq (ou configure no ambiente)
os.environ["GROQ_API_KEY"] = "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"

# Inicialize o modelo da Groq (exemplo: Llama 3)
llm = ChatGroq(model_name="llama3-70b", api_key=os.environ["GROQ_API_KEY"])

def gerar_documentacao():
    prompt = "Analise todo o código deste repositório e gere uma documentação detalhada e concisa."

    agente = Agent(
        role="Documentador",
        goal="Gerar documentação completa do código",
        backstory="Você é um especialista em análise e documentação de código.",
        tools=[PythonREPLTool()],  # Ferramenta opcional para análise de código
        verbose=True,
        allow_delegation=False,
        llm=llm  # Especifica que este agente usará a API da Groq
    )

    resposta = agente.run(prompt)
    return resposta

def atualizar_readme(conteudo):
    arquivo = "README.md"
    try:
        with open(arquivo, "r", encoding="utf-8") as f:
            linhas = f.readlines()
    except FileNotFoundError:
        print(f"{arquivo} não encontrado.")
        return

    try:
        inicio = linhas.index("<!-- DOC START -->\n") + 1
        fim = linhas.index("<!-- DOC END -->\n")
    except ValueError:
        print("Tags <!-- DOC START --> e <!-- DOC END --> não encontradas no README.md")
        return

    novas_linhas = linhas[:inicio] + [conteudo + "\n"] + linhas[fim:]
    with open(arquivo, "w", encoding="utf-8") as f:
        f.writelines(novas_linhas)

if __name__ == "__main__":
    docs = gerar_documentacao()
    atualizar_readme(docs)
"""


def add_doc(repo, path, content, message="Update documentation"):
    branch = repo.default_branch  # Usa a branch padrão do repositório
    try:
        repo.get_contents(path, ref=branch)
        print(f"{path} já existe no branch {branch} de {repo.full_name}.")
    except GithubException as e:
        if e.status == 404:
            print(f"Arquivo {path} não encontrado no branch {
                  branch} de {repo.full_name}. Criando arquivo...")
            try:
                repo.create_file(
                    path=path,
                    message=message,
                    content=content,
                    branch=branch
                )
                print(f"Arquivo {path} criado com sucesso no branch {
                      branch} de {repo.full_name}.")
            except Exception as ex:
                print(f"Erro ao criar arquivo {
                      path} em {repo.full_name}: {ex}")
        else:
            print(f"Erro ao obter conteúdo do arquivo {
                  path} em {repo.full_name}: {e}")


# Itera por todos os repositórios do usuário
for repo in g_user.get_repos():
    print(f"Verificando repositório: {repo.name}")
    add_doc(repo, workflow_path, workflow_content,
            message="Adicionando workflow de atualização de documentação")
    add_doc(repo, script_path, script_content,
            message="Adicionando script de atualização de documentação")
