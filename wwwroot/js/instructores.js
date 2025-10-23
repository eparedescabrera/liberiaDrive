// ============================
// MODALES AJAX CRUD INSTRUCTORES
// ============================

// 🔹 Crear Instructor
function abrirModalCrearInstructor() {
    $("#tituloModal").html(`<i class="bi bi-person-plus"></i> Nuevo Instructor`);
    $("#contenidoModal").html(`
        <div class="text-center py-4">
            <div class="spinner-border text-primary"></div>
            <p>Cargando formulario...</p>
        </div>`);

    const modal = new bootstrap.Modal(document.getElementById("modalInstructor"));
    modal.show();

    $.get("/Instructores/Create", function (data) {
        $("#contenidoModal").html(data);
    });
}

// 🔹 Editar Instructor
function abrirModalEditarInstructor(id) {
    $("#tituloModal").html(`<i class="bi bi-pencil-square text-warning"></i> Editar Instructor`);
    $("#contenidoModal").html(`
        <div class="text-center py-4">
            <div class="spinner-border text-warning"></div>
            <p>Cargando datos...</p>
        </div>`);

    const modal = new bootstrap.Modal(document.getElementById("modalInstructor"));
    modal.show();

    $.get(`/Instructores/Edit/${id}`, function (data) {
        $("#contenidoModal").html(data);
    });
}

// 🔹 Ver detalles
function abrirModalDetallesInstructor(id) {
    $("#tituloModal").html(`<i class="bi bi-eye"></i> Detalles del Instructor`);
    $("#contenidoModal").html(`
        <div class="text-center py-4">
            <div class="spinner-border text-info"></div>
            <p>Cargando detalles...</p>
        </div>`);

    const modal = new bootstrap.Modal(document.getElementById("modalInstructor"));
    modal.show();

    $.get(`/Instructores/Details/${id}`, function (data) {
        $("#contenidoModal").html(data);
    });
}

// 🔹 Eliminar Instructor
function abrirModalEliminarInstructor(id) {
    $("#tituloModal").html(`<i class="bi bi-trash3 text-danger"></i> Eliminar Instructor`);
    $("#contenidoModal").html(`
        <div class="text-center py-4">
            <div class="spinner-border text-danger"></div>
            <p>Cargando información...</p>
        </div>`);

    const modal = new bootstrap.Modal(document.getElementById("modalInstructor"));
    modal.show();

    $.get(`/Instructores/Delete/${id}`, function (data) {
        $("#contenidoModal").html(data);
    });
}

// ============================
// SUBMIT (AJAX + SWEETALERT)
// ============================

// CREATE
$(document).off("submit", "#formCrearInstructor").on("submit", "#formCrearInstructor", function (e) {
    e.preventDefault();
    $.post("/Instructores/Create", $(this).serialize())
        .done(resp => {
            if (resp.success) {
                Swal.fire({
                    icon: "success",
                    title: "Instructor agregado",
                    showConfirmButton: false,
                    timer: 1500
                });
                $("#modalInstructor").modal("hide");
                setTimeout(() => location.reload(), 1200);
            } else {
                Swal.fire("Error", resp.message, "error");
            }
        });
});

// EDIT
$(document).off("submit", "#formEditarInstructor").on("submit", "#formEditarInstructor", function (e) {
    e.preventDefault();
    $.post("/Instructores/Edit", $(this).serialize())
        .done(resp => {
            if (resp.success) {
                Swal.fire({
                    icon: "success",
                    title: "Instructor actualizado",
                    showConfirmButton: false,
                    timer: 1500
                });
                $("#modalInstructor").modal("hide");
                setTimeout(() => location.reload(), 1200);
            } else {
                Swal.fire("Error", resp.message, "error");
            }
        });
});

// DELETE
$(document).off("submit", "#formEliminarInstructor").on("submit", "#formEliminarInstructor", function (e) {
    e.preventDefault();
    const id = $("#IdInstructor").val();

    Swal.fire({
        title: "¿Eliminar instructor?",
        text: "Esta acción no se puede deshacer.",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#d33",
        cancelButtonColor: "#6c757d",
        confirmButtonText: "Sí, eliminar",
        cancelButtonText: "Cancelar"
    }).then((result) => {
        if (result.isConfirmed) {
            $.post("/Instructores/DeleteConfirmed", { IdInstructor: id })
                .done(resp => {
                    if (resp.success) {
                        Swal.fire({
                            icon: "success",
                            title: "Instructor eliminado",
                            showConfirmButton: false,
                            timer: 1500
                        });
                        $("#modalInstructor").modal("hide");
                        setTimeout(() => location.reload(), 1200);
                    } else {
                        Swal.fire("Error", resp.message, "error");
                    }
                });
        }
    });
});
