// ================================
// CLIENTES - FUNCIONES MODALES
// ================================
// ================================
// NUEVO CLIENTE (Modal AJAX + SweetAlert)
// ================================
function abrirModalCrear() {
    // Cambiar título del modal y mostrar animación
    $("#tituloModal").text("Registrar Nuevo Cliente");
    $("#contenidoModal").html(`
        <div class="text-center py-4">
            <div class="spinner-border text-primary" role="status">
                <span class="visually-hidden">Cargando...</span>
            </div>
        </div>`);
    $("#modalCliente").modal("show");

    // Cargar el formulario parcial desde el controlador
    $.get("/Clientes/Create")
        .done(function (data) {
            // Insertar el partial cargado en el modal
            $("#contenidoModal").html(data);

            // Manejar el evento Submit del formulario
            $("#formNuevoCliente").on("submit", function (e) {
                e.preventDefault();
                var form = $(this);

                $.ajax({
                    type: "POST",
                    url: form.attr("action"),
                    data: form.serialize(),
                    success: function (response) {
                        if (response.success) {
                            $("#modalCliente").modal("hide");

                            Swal.fire({
                                title: "✅ Cliente registrado",
                                text: "El cliente se agregó correctamente.",
                                icon: "success",
                                confirmButtonText: "Aceptar"
                            }).then(() => {
                                location.reload(); // Recarga la tabla
                            });
                        } else {
                            Swal.fire({
                                title: "Error",
                                text: response.message || "No se pudo registrar el cliente.",
                                icon: "error"
                            });
                        }
                    },
                    error: function () {
                        Swal.fire({
                            title: "Error inesperado",
                            text: "No se pudo procesar la solicitud.",
                            icon: "error"
                        });
                    }
                });
            });
        })
        .fail(function () {
            $("#contenidoModal").html("<div class='text-danger text-center py-4'>❌ Error al cargar el formulario.</div>");
        });
}
// ================================
// DETALLES CLIENTE
// ================================
function abrirModalDetalles(id) {
    $("#tituloModal").text("Detalles del Cliente");
    $("#contenidoModal").html(`
        <div class="text-center py-4">
            <div class="spinner-border text-primary" role="status">
                <span class="visually-hidden">Cargando...</span>
            </div>
        </div>`);
    $("#modalCliente").modal("show");

    $.get("/Clientes/Details/" + id)
        .done(function (data) {
            $("#contenidoModal").html(data);
        })
        .fail(function () {
            $("#contenidoModal").html("<div class='text-danger text-center py-4'>❌ Error al cargar los detalles.</div>");
        });
}

// ================================
// ELIMINAR CLIENTE (SweetAlert + AJAX)
// ================================
function eliminarCliente(id) {
    Swal.fire({
        title: "¿Eliminar cliente?",
        text: "Esta acción no se puede deshacer.",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#d33",
        cancelButtonColor: "#3085d6",
        confirmButtonText: "Sí, eliminar",
        cancelButtonText: "Cancelar"
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: "/Clientes/Delete/" + id,
                type: "POST",
                success: function () {
                    Swal.fire({
                        title: "Eliminado",
                        text: "El cliente fue eliminado correctamente.",
                        icon: "success",
                        confirmButtonColor: "#3085d6"
                    }).then(() => {
                        location.reload();
                    });
                },
                error: function () {
                    Swal.fire({
                        title: "Error",
                        text: "No se pudo eliminar el cliente.",
                        icon: "error"
                    });
                }
            });
        }
    });
}

// 🔹 Abrir modal para editar
function abrirModalEditar(id) {
    $("#tituloModal").text("Editar Cliente");
    $("#contenidoModal").html("<div class='text-center py-5 text-muted'>Cargando...</div>");
    $("#modalCliente").modal("show");

    $.get("/Clientes/Edit/" + id)
        .done(function (data) {
            $("#contenidoModal").html(data);

            // ✅ Reasignar el evento submit dentro del modal
            $("#formEditarCliente").on("submit", function (e) {
                e.preventDefault();

                var form = $(this);
                $.ajax({
                    type: "POST",
                    url: form.attr("action"),
                    data: form.serialize(),
                    success: function (response) {
                        if (response.success) {
                            $("#modalCliente").modal("hide");
                            setTimeout(() => {
                                Swal.fire({
                                    title: "Cliente actualizado",
                                    text: "Los cambios se guardaron correctamente.",
                                    icon: "success",
                                    confirmButtonText: "Aceptar"
                                }).then(() => location.reload());
                            }, 400);

                        } else {
                            // ⚠️ Mostrar error devuelto
                            Swal.fire({
                                title: "Error",
                                text: response.message || "No se pudo actualizar el cliente.",
                                icon: "error",
                                confirmButtonText: "Cerrar",
                                confirmButtonColor: "#d33"
                            });
                        }
                    },
                    error: function () {
                        Swal.fire({
                            title: "Error",
                            text: "Error inesperado al procesar la solicitud.",
                            icon: "error",
                            confirmButtonText: "Cerrar",
                            confirmButtonColor: "#d33"
                        });
                    }
                });
            });
        })
        .fail(function () {
            $("#contenidoModal").html("<div class='text-danger text-center py-4'>❌ Error al cargar los datos.</div>");
        });
}

// 🔹 Abrir modal para ver detalles
function abrirModalDetalles(id) {
    $("#tituloModal").text("Detalles del Cliente");
    $("#contenidoModal").html("<div class='text-center py-5 text-muted'>Cargando...</div>");
    $("#modalCliente").modal("show");

    $.get("/Clientes/Details/" + id)
        .done(function (data) {
            $("#contenidoModal").html(data);
        })
        .fail(function () {
            $("#contenidoModal").html("<div class='text-danger text-center py-4'>❌ Error al cargar los datos.</div>");
        });
}
