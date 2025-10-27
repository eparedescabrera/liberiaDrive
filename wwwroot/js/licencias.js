function abrirModalCrear() {
    $("#tituloModal").text("Nueva Licencia");
    $("#contenidoModal").load("/Licencias/Create", function () {
        $("#modalLicencia").modal("show");
    });
}

function abrirModalEditar(id) {
    $("#tituloModal").text("Editar Licencia");
    $("#contenidoModal").load("/Licencias/Edit/" + id, function () {
        $("#modalLicencia").modal("show");
    });
}

function abrirModalEliminar(id) {
    $("#tituloModal").text("Eliminar Licencia");
    $("#contenidoModal").load("/Licencias/Delete/" + id, function () {
        $("#modalLicencia").modal("show");
    });
}

// ✅ Crear
$(document).on("submit", "#formCrearLicencia", function (e) {
    e.preventDefault();
    $.post("/Licencias/Create", $(this).serialize(), resp => {
        if (resp.success) {
            Swal.fire("¡Guardado!", "Licencia registrada.", "success");
            $("#modalLicencia").modal("hide");
            setTimeout(() => location.reload(), 1200);
        }
    });
});

// ✅ Editar
$(document).on("submit", "#formEditarLicencia", function (e) {
    e.preventDefault();
    $.post("/Licencias/Edit", $(this).serialize(), resp => {
        if (resp.success) {
            Swal.fire("¡Actualizado!", "Cambios guardados.", "success");
            $("#modalLicencia").modal("hide");
            setTimeout(() => location.reload(), 1200);
        }
    });
});

// ✅ Eliminar
$(document).on("submit", "#formEliminarLicencia", function (e) {
    e.preventDefault();
    let id = $("#IdLicencia").val();
    $.post("/Licencias/DeleteConfirmed", { id: id }, resp => {
        if (resp.success) {
            Swal.fire("¡Eliminado!", "Licencia eliminada.", "success");
            $("#modalLicencia").modal("hide");
            setTimeout(() => location.reload(), 1200);
        }
    });
});
