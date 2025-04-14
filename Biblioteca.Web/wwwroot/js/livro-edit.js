// Pré-visualização da imagem

function previewImage(event) {
    var reader = new FileReader();
    reader.onload = function () {
        var output = document.getElementById('imagePreview');
        output.src = reader.result;
    };
    if (event.target.files.length > 0) {
        reader.readAsDataURL(event.target.files[0]);
    }
}

// Adicionar trechos
function adicionarTrecho() {
    var container = document.getElementById('trechosContainer');
    var trechoDiv = document.createElement("div");
    trechoDiv.className = "input-group mb-2 trecho-item";

    var novoInput = document.createElement("input");
    novoInput.type = "text";
    novoInput.name = "TrechosFavoritos[]";
    novoInput.className = "form-control";
    novoInput.placeholder = "Adicione um trecho";

    var botaoRemover = document.createElement("button");
    botaoRemover.type = "button";
    botaoRemover.className = "btn btn-danger";
    botaoRemover.textContent = "X";
    botaoRemover.onclick = function () {
        removerTrecho(botaoRemover);
    };

    trechoDiv.appendChild(novoInput);
    trechoDiv.appendChild(botaoRemover);
    container.appendChild(trechoDiv);
}

function removerTrecho(botao) {
    botao.parentElement.remove();
}