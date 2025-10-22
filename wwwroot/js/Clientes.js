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
// 🧩 Función moderna para abrir el modal de eliminación
function abrirModalEliminar(id) {
    // Efecto visual de carga
    $("#tituloModal").html(`<i class="bi bi-trash3 text-danger"></i> Eliminar Cliente`);
    $("#contenidoModal").html(`
        <div class="d-flex flex-column align-items-center justify-content-center py-5 text-center text-muted">
            <div class="spinner-border text-danger mb-3" style="width: 3rem; height: 3rem;" role="status"></div>
            <p class="fw-semibold">Cargando información del cliente...</p>
        </div>
    `);

    // Mostrar el modal
    const modal = new bootstrap.Modal(document.getElementById("modalCliente"));
    modal.show();

    // Solicitud AJAX para cargar los datos del cliente
    $.ajax({
        url: `/Clientes/Delete/${id}`,
        type: "GET",
        success: function (data) {
            // Si viene contenido válido (HTML del partial)
            if (data && data.trim().length > 0) {
                $("#contenidoModal").hide().html(data).fadeIn(300);
            } else {
                mostrarErrorModal("No se pudo cargar el cliente seleccionado.");
            }
        },
        error: function (xhr) {
            let msg = xhr.responseText || "Error al obtener los datos del cliente.";
            mostrarErrorModal(msg);
        }
    });
}

// 🎨 Función auxiliar para mostrar errores elegantes dentro del modal
function mostrarErrorModal(mensaje) {
    $("#contenidoModal").html(`
        <div class="text-center py-5">
            <i class="bi bi-exclamation-triangle text-danger display-4"></i>
            <p class="mt-3 fw-bold text-danger">${mensaje}</p>
            <button class="btn btn-outline-secondary mt-2" data-bs-dismiss="modal">
                <i class="bi bi-x-circle"></i> Cerrar
            </button>
        </div>
    `);
}
