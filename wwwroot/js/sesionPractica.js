$(document).ready(function () {

    // =====================================================
    // ✅ NUEVA SESIÓN PRÁCTICA
    // =====================================================
    $("#btnNuevaSesion").click(function () {
        $("#tituloModal").text("Registrar Sesión Práctica");
        $("#contenidoModal").load("/SesionPractica/Create", function () {
            $("#modalSesion").modal("show");
        });
    });

    // =====================================================
    // ✅ EDITAR SESIÓN PRÁCTICA
    // =====================================================
    $(document).on("click", ".btn-editar-sesion", function () {
        const id = $(this).data("id");
        $("#tituloModal").text("Editar Sesión Práctica");
        $("#contenidoModal").load("/SesionPractica/Edit/" + id, function () {
            $("#modalSesion").modal("show");
        });
    });

    // =====================================================
    // ✅ ELIMINAR SESIÓN PRÁCTICA
    // =====================================================
    $(document).on("click", ".btn-eliminar-sesion", function () {
        const id = $(this).data("id");
        $("#tituloModal").text("Eliminar Sesión Práctica");
        $("#contenidoModal").load("/SesionPractica/Delete/" + id, function () {
            $("#modalSesion").modal("show");
        });
    });

    // =====================================================
    // ✅ GUARDAR NUEVA SESIÓN (POST)
    // =====================================================
    $(document).on("submit", "#formCrearSesion", function (e) {
        e.preventDefault();
        const data = $(this).serialize();

        $.post("/SesionPractica/Create", data, function (resp) {
            mostrarResultado(resp, "Sesión registrada correctamente.");
        });
    });

    // =====================================================
    // ✅ ACTUALIZAR SESIÓN (POST)
    // =====================================================
    $(document).on("submit", "#formEditarSesion", function (e) {
        e.preventDefault();
        const data = $(this).serialize();

        $.post("/SesionPractica/Edit", data, function (resp) {
            mostrarResultado(resp, "Sesión actualizada correctamente.");
        });
    });

    // =====================================================
    // ✅ CONFIRMAR ELIMINACIÓN (POST)
    // =====================================================
    $(document).on("submit", "#formEliminarSesion", function (e) {
        e.preventDefault();
        const data = $(this).serialize();

        $.post("/SesionPractica/DeleteConfirmed", data, function (resp) {
            mostrarResultado(resp, "Sesión eliminada correctamente.");
        });
    });

    // =====================================================
    // ✅ ALERTAS GLOBALES (SweetAlert)
    // =====================================================
    window.mostrarResultado = function (resp, mensajeExito) {
        if (resp.success) {
            Swal.fire({
                icon: "success",
                title: "Éxito",
                text: mensajeExito,
                timer: 1800,
                showConfirmButton: false
            }).then(() => location.reload());
        } else {
            Swal.fire({
                icon: "error",
                title: "Error",
                text: resp.message
            });
        }
    };

    // =====================================================
    // ✅ CONFIGURAR DATATABLE
    // =====================================================
    $("#tablaSesiones").DataTable({
        language: { url: "//cdn.datatables.net/plug-ins/1.13.1/i18n/es-ES.json" },
        pageLength: 8,
        lengthChange: false
    });
});
