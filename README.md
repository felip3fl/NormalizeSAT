<h3 align="center"> <img src="https://raw.githubusercontent.com/felip3fl/felip3fl/d3fc7831c647a88ff79c6d6d8f1041014bb71028/Material/FL/FLmetroDefault.svg" width="160px" align="center" ><BR><BR>NormalizeSAT
<BR><BR>
  <img src="https://img.shields.io/badge/STATUS-FINALIZADO-ff265c" alt="Status: FINALIZADO">
  <img src="https://img.shields.io/badge/PROJECT%20VERSION-1.0.0-ff262f" alt="Project version: 1.0.0">
<BR><BR>
</h3>

## 📃 Descrição do projeto

<p align="justify">
O <b>NormalizeSAT</b> é uma aplicação de console desenvolvida em C# (.NET 8) criada para automatizar a organização e a renomeação em lote de arquivos PDF de documentos fiscais eletrônicos (como cupons fiscais SAT, NFC-e e DANFE). A ferramenta analisa automaticamente os arquivos PDF de um diretório informado, extrai o nome da empresa emissora e a data e hora de emissão diretamente do conteúdo do documento utilizando a biblioteca <b>PdfPig</b>, e renomeia cada arquivo em um padrão limpo, padronizado e cronológico (<code>yyMMdd HHmm Nome Da Empresa.pdf</code>), evitando sobrescritas e caracteres inválidos.
</p>

<br>

## :hammer: Funcionalidades do projeto

- `Processamento em lote por prefixo`: localiza e processa automaticamente todos os arquivos PDF iniciados com `DOCUMENTO` ou `Consulta` dentro da pasta informada.
- `Extração inteligente do nome da empresa`: reconstrói as linhas da primeira página do PDF agrupando palavras por coordenadas espaciais (eixo vertical e horizontal) para capturar a primeira linha válida, contando também com fallback para extração de texto bruto.
- `Extração de data e hora de emissão`: percorre as páginas do documento buscando o campo `Emissão: DD/MM/YYYY HH:mm:ss` via expressão regular (com suporte tanto a caminho de arquivo quanto a fluxo de bytes).
- `Padronização de nomenclatura`: renomeia os arquivos no formato `yyMMdd HHmm Nome Da Empresa.pdf`, aplicando formatação em *Title Case* ao nome do estabelecimento para facilitar a ordenação cronológica e a leitura.
- `Sanitização de nomes de arquivo`: substitui caracteres inválidos para Windows, Linux e macOS por `_`, remove espaços duplicados e limita o tamanho do nome a 150 caracteres.
- `Prevenção de sobrescrita`: detecta arquivos já existentes com o mesmo nome e acrescenta automaticamente sufixos numéricos sequenciais como `(2)`, `(3)`, etc., além de ignorar arquivos que já estejam com o nome correto.
- `Execução via linha de comando ou modo interativo`: permite informar o diretório dos PDFs diretamente como argumento na execução ou digitá-lo no console quando solicitado.

<br>

## 📁 Acesso ao projeto

Você pode acessar o código-fonte do projeto diretamente na pasta `Source` deste repositório no GitHub ou cloná-lo para sua máquina:

- **Repositório**: [https://github.com/felip3fl/NormalizeSAT](https://github.com/felip3fl/NormalizeSAT)

```bash
git clone https://github.com/felip3fl/NormalizeSAT.git
```

<br>

## 🛠️ Abrir e rodar o projeto

### Pré-requisitos

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) instalado
- [Visual Studio 2022](https://visualstudio.microsoft.com/) ou [Visual Studio Code](https://code.visualstudio.com/) (opcional)

### Executando pelo terminal (.NET CLI)

1. Acesse a pasta com o código-fonte do projeto:
```bash
cd NormalizeSAT/Source
```

2. Restaure as dependências e compile o projeto:
```bash
dotnet restore
dotnet build
```

3. Execute a aplicação passando o caminho da pasta contendo os PDFs como argumento:
```bash
dotnet run -- "C:\caminho\para\pasta\com\pdfs"
```

Ou execute em modo interativo e informe o caminho quando solicitado:
```bash
dotnet run
```

### Executando pelo Visual Studio

1. Abra o arquivo de solução `Source/NormalizeSAT.sln` no Visual Studio.
2. Caso deseje configurar uma pasta padrão para depuração, ajuste o campo `commandLineArgs` em `Source/Properties/launchSettings.json`.
3. Pressione `F5` para compilar e executar o projeto.

### Gerando o executável (Publish)

Para gerar o executável `NormalizeSAT.exe` em modo *Release* para Windows (`win-x64`):
```bash
dotnet publish -c Release -r win-x64
```

<br>  

## ✔️ Técnicas e tecnologias utilizadas
<p align="justify">
<img width="80" src="https://cdn.jsdelivr.net/gh/devicons/devicon/icons/csharp/csharp-original.svg" alt="C#">
&nbsp;&nbsp;<img width="80" src="https://cdn.jsdelivr.net/gh/devicons/devicon/icons/dotnetcore/dotnetcore-original.svg" alt=".NET">
&nbsp;&nbsp;<img width="80" src="https://cdn.jsdelivr.net/gh/devicons/devicon/icons/git/git-original.svg" alt="Git">
&nbsp;&nbsp;<img width="80" src="https://raw.githubusercontent.com/felip3fl/felip3fl/1a6a66b6a143aab342cf2df18f56d8c1c7e6c8fb/Material/Icon/visual-studio.svg" alt="Visual Studio">
&nbsp;&nbsp;<img width="80" src="https://raw.githubusercontent.com/felip3fl/felip3fl/1a6a66b6a143aab342cf2df18f56d8c1c7e6c8fb/Material/Icon/vscode.svg" alt="Visual Studio Code">
</p>

- `C#`
- `.NET 8.0` (Aplicação de Console)
- `PdfPig (v0.1.9)` — biblioteca C# pura para leitura e extração de texto e coordenadas de arquivos PDF, sem dependências nativas
- `Expressões Regulares (Regex)` — extração de data/hora de emissão e sanitização de nomes de arquivos
- `LINQ` — agrupamento e ordenação de palavras por posicionamento na página do PDF
- `Git`
- `Visual Studio` e `Visual Studio Code`

<br>  

## Autor

| [<img src="https://github.com/felip3fl/felip3fl/blob/main/Material/Nick/nick1.jpg?raw=true" width=115><br><sub>Felipe Lima</sub>](https://github.com/felip3fl) | 
| :---: 

<br>

## Licença

<i><h6>Todos os direitos reservados. O autor permite apenas a visualização do código, sendo proibida qualquer utilização do mesmo, no todo ou em parte.</h6></i>
