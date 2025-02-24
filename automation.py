#!/usr/bin/env python3
import os
from crewai import Agent

def gerar_documentacao():
    prompt = "Analise todo o código deste repositório e gere uma documentação detalhada e concisa."
    agente = Agent(
        role="Documentador",
        goal="Gerar documentação completa do código",
        backstory="Você é um especialista em análise e documentação de código.",
        verbose=True,
        allow_delegation=False,
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
