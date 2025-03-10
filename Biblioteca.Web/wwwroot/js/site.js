// Função para abrir a modal delete de livro
$('.btn-produto-delete').click(function () {
    var id = $(this).data('id');
    var titulo = $(this).data('titulo');
    var deleteUrl = $(this).data('url');

    $('#modalProdutoDelete').modal('show');  
    $('#titulo').text(titulo);  

    $('#confirmDelete').off('click').on('click', function () {
        $.ajax({
            url: deleteUrl + '/' + id,
            type: 'DELETE',
            success: function (data) {
                console.log('Livro excluído com sucesso');
                // Redirecionar para a página "Index" após a exclusão
                window.location.href = "/Livro/Index"; 
            },
            error: function (xhr, status, error) {
                console.error(xhr.responseText);
                window.location.href = "/Livro/Index";
            }
        });
    });
});