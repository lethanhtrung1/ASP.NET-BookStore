var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#dataTable').DataTable({
        "ajax": { url: '/admin/product/getall' },
        "columns": [
            { data: 'title', "width": "20%" },
            { data: 'isbn', "width": "15%" },
            { data: 'listPrice', "width": "10%" },
            { data: 'author', "width": "15%" },
            { data: 'category.name', "width": "15%" },
            {
                data: 'id',
                "render": function (data) {
                    return `
                        <div class="w-75 btn-group" role="group">
							<a href="/Admin/Product/Upsert?productId=${data}" class="btn btn-outline-success mx-1" style="min-width: 120px">
								<i class="bi bi-pencil-square"></i>
							</a>
							<a onClick=Delete("/admin/product/delete/${data}") class="btn btn-outline-danger mx-1" style="min-width: 120px">
								<i class="bi bi-trash"></i>
							</a>
						</div>
                    `;
                },
                "width": "25%"
            },
        ],
    });
}

function Delete(url) {
    Swal.fire({
        title: "Are you sure?",
        text: "You won't be able to revert this!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Yes, delete it!"
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: "DELETE",
                success: function (data) {
                    dataTable.ajax.reload();
                    toastr.success(data.message);

                }
            })
        }
    });
}