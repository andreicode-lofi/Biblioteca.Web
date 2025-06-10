    // Pré-visualização da imagem antes de enviar a requisição
    function previewImage(event) {
        var reader = new FileReader();
        reader.onload = function () {
            var output = document.getElementById('imagePreview');
            output.src = reader.result;
        }
        if (event.target.files.length > 0) {
            reader.readAsDataURL(event.target.files[0]);
        }
    }

    // Adicionar mais trechos favoritos dinamicamente
    function adicionarTrecho() {
        var container = document.getElementById('trechosContainer');
        var div = document.createElement("div");
        div.className = "input-group mb-2";
        
        var novoInput = document.createElement("input");
        novoInput.type = "text";
        novoInput.name = "TrechosFavoritos[]";
        novoInput.className = "form-control";
        novoInput.placeholder = "Adicione um trecho";
        
        var botaoRemover = document.createElement("button");
        botaoRemover.type = "button";
        botaoRemover.className = "btn btn-danger";
        botaoRemover.textContent = "X";
        botaoRemover.onclick = function () { removerTrecho(botaoRemover); };
        
        div.appendChild(novoInput);
        div.appendChild(botaoRemover);
        container.appendChild(div);
    }

    function removerTrecho(botao) {
        botao.parentElement.remove();
    }


//enviando dados do googleBooks para o endpoint create livro.
document.addEventListener("DOMContentLoaded", function () {
    const nomeInput = document.getElementById("Nome");
    const autorInput = document.getElementById("Autor");
    const generoInput = document.getElementById("Genero");
    const anoInput = document.getElementById("ano");
    const sinopseInput = document.getElementById("Sinopse");
    const paginasInput = document.getElementById("NumeroPaginas");
    const imagemInput = document.getElementById("imageInput");
    const imagePreview = document.getElementById("imagePreview");
    const imagemHidden = document.getElementById("Imagem"); // <- campo oculto para o link da imagem

    const botaoBuscar = document.getElementById("btnBuscarLivro");

    if (botaoBuscar) {
        botaoBuscar.addEventListener("click", async function () {
            const nomeLivro = nomeInput.value;

            if (!nomeLivro.trim()) {
                alert("Digite o nome do livro para buscar.");
                return;
            }

            try {
                const response = await fetch(`/Livro/BuscarPorNome?nome=${encodeURIComponent(nomeLivro)}`);

                if (!response.ok) {
                    throw new Error("Livro não encontrado.");
                }

                const data = await response.json();

                autorInput.value = data.autor || "";
                generoInput.value = data.genero || "";
                anoInput.value = data.ano || "";
                sinopseInput.value = data.sinopese || "";
                paginasInput.value = data.numeroPaginas || "";

                // Exibe a imagem se vier da API e atualiza o campo hidden
                if (data.imagem) {
                    imagePreview.src = data.imagem;
                    imagemHidden.value = data.imagem; // <- ESSENCIAL para enviar ao controller
                } else {
                    imagePreview.src = "";
                    imagemHidden.value = "";
                }

            } catch (error) {
                alert("Livro não encontradooo. Preencha os dados manualmente.");
                console.error(error);
            }
        });
    }
});