$(document).ready(function () {

    // =====================================================
    // ✅ CREAR RESULTADO
    // =====================================================
    $("#btnNuevoResultado").off("click").on("click", function () {
        $("#tituloModal").text("Registrar Resultado de Examen");
        $("#contenidoModal").load("/ResultadoExamen/Create", function () {
            $("#modalResultado").modal("show");
        });
    });

    // =====================================================
    // ✅ EDITAR RESULTADO
    // =====================================================
    $(document).off("click", ".btn-editar-resultado").on("click", ".btn-editar-resultado", function () {
        const id = $(this).data("id");
        $("#tituloModal").text("Editar Resultado");
        $("#contenidoModal").load("/ResultadoExamen/Edit/" + id, function () {
            $("#modalResultado").modal("show");
        });
    });

    // =====================================================
    // ✅ ELIMINAR RESULTADO
    // =====================================================
    $(document).off("click", ".btn-eliminar-resultado").on("click", ".btn-eliminar-resultado", function () {
        const id = $(this).data("id");
        $("#tituloModal").text("Eliminar Resultado");
        $("#contenidoModal").load("/ResultadoExamen/Delete/" + id, function () {
            $("#modalResultado").modal("show");
        });
    });

    // =====================================================
    // ✅ GUARDAR NUEVO RESULTADO (POST)
    // =====================================================
    $(document).off("submit", "#formCrearResultado").on("submit", "#formCrearResultado", function (e) {
        e.preventDefault();
        const data = $(this).serialize();

        $.post("/ResultadoExamen/Create", data)
            .done(function (resp) {
                if (resp.success) {
                    mostrarResultado(resp, "Resultado registrado correctamente.");
                } else {
                    Swal.fire({
                        icon: "warning",
                        title: "Advertencia",
                        text: resp.message
                    });
                }
            })
            .fail(function () {
                Swal.fire({
                    icon: "error",
                    title: "Error",
                    text: "Ocurrió un problema al registrar el resultado."
                });
            });
    });

    // =====================================================
    // ✅ ACTUALIZAR RESULTADO (POST)
    // =====================================================
    $(document).off("submit", "#formEditarResultado").on("submit", "#formEditarResultado", function (e) {
        e.preventDefault();
        const data = $(this).serialize();

        $.post("/ResultadoExamen/Edit", data)
            .done(function (resp) {
                mostrarResultado(resp, "Resultado actualizado correctamente.");
            })
            .fail(function () {
                Swal.fire({
                    icon: "error",
                    title: "Error",
                    text: "Ocurrió un problema al actualizar el resultado."
                });
            });
    });

    // =====================================================
    // ✅ CONFIRMAR ELIMINACIÓN (POST)
    // =====================================================
    $(document).off("submit", "#formEliminarResultado").on("submit", "#formEliminarResultado", function (e) {
        e.preventDefault();
        const data = $(this).serialize();

        $.post("/ResultadoExamen/DeleteConfirmed", data)
            .done(function (resp) {
                mostrarResultado(resp, "Resultado eliminado correctamente.");
            })
            .fail(function () {
                Swal.fire({
                    icon: "error",
                    title: "Error",
                    text: "Ocurrió un problema al eliminar el resultado."
                });
            });
    });

    // =====================================================
    // ✅ FUNCIÓN GLOBAL DE ALERTAS
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

});
