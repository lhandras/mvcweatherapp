$(function () {
    $(document).on('click', '.showModalButton', function () {
        $('#cityModal').modal('show')
            .find('#modalContent')
            .load($(this).attr('value'));
    });

    $(document).on('click', '.showDetailModalButton', function () {
        $('#detailModal').modal('show')
            .find('#detailModalContent')
            .load($(this).attr('value'));

        document.getElementById('detailModalHeaderTitle').innerHTML = $(this).attr('title');
    });
});