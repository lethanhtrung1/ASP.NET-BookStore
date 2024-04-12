var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#dataTable').DataTable({
        "ajax": { url: '/admin/user/getall' },
        "columns": [
            { data: 'name', "width": "20%" },
            { data: 'email', "width": "20%" },
            { data: 'phoneNumber', "width": "15%" },
            { data: 'company.name', "width": "15%" },
            { data: 'role', "width": "10%" },
            {
                data: { id: "id", lockoutEnd: "lockoutEnd" },
                "render": function (data) {
                    var today = new Date().getTime();
                    var lockout = new Date(data.lockoutEnd).getTime();

                    if (lockout > today) {
                        return `
                            <div class="w-75 btn-group" role="group">
							    <a onclick=LockUnlock('${data.id}') class="btn btn-outline-danger mx-1" style="min-width: 120px">
								    <i class="bi bi-lock-fill"></i>
							    </a>
                                <a href="/admin/user/RoleManagement?userId=${data.id}" class="btn btn-outline-danger mx-1" style="min-width: 120px">
								    <i class="bi bi-pencil-square"></i>
							    </a>
						    </div>
                        `;
                    } else {
                        return `
                            <div class="w-75 btn-group" role="group">
                                <a onclick=LockUnlock('${data.id}') class="btn btn-outline-success mx-1" style="min-width: 120px">
								    <i class="bi bi-unlock-fill"></i>
							    </a>
                                <a href="/admin/user/RoleManagement?userId=${data.id}" class="btn btn-outline-danger mx-1" style="min-width: 120px">
								    <i class="bi bi-pencil-square"></i>
							    </a>
						    </div>
                        `;
                    }
                },
                "width": "20%"
            },
        ],
    });
}

function LockUnlock(id) {
    $.ajax({
        type: "POST",
        url: '/Admin/User/LockUnlock',
        data: JSON.stringify(id),
        contentType: "application/json",
        success: function (data) {
            if (data.success) {
                toastr.success(data.message);
                dataTable.ajax.reload();
            }
        }
    });
}