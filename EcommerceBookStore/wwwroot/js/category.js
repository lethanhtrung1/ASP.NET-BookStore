var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#categoryDataTable').DataTable({
        "ajax": { url: '/admin/category/getall' },
        "columns": [
            { data: 'name', "width": "30%" },
            { data: 'displayOrder', "width": "20%" },
            {
                data: 'id',
                "render": function (data) {
                    return `
                        <div class="w-75 btn-group" role="group">
                            <a href="/Admin/Category/Edit?categoryId=${data}" class="btn btn-outline-success mx-1" style="min-width: 120px">
							    <i class="bi bi-pencil-square"></i>
						    </a>
							<a href="/Admin/Category/Delete?categoryId=${data}" class="btn btn-outline-danger mx-1" style="min-width: 120px">
								<i class="bi bi-trash"></i>
							</a>
                        </div>
                    `
                },
                "width": "50%"
            },
        ]
    });
}