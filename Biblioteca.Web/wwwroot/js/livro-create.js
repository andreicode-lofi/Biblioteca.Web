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