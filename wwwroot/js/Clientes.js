// ================================
// CLIENTES - FUNCIONES MODALES
// ================================

// 🧠 Helper: abre modal usando Bootstrap 5 (sin errores de backdrop)
function mostrarModal() {
    const modalElement = document.getElementById("modalCliente");
    const modal = bootstrap.Modal.getOrCreateInstance(modalElement);
    modal.show();
}

function cerrarModal() {
    const modalElement = document.getElementById("modalCliente");
    const modal = bootstrap.Modal.getInstance(modalElement);
    if (modal) modal.hide();
}

// ================================
// NUEVO CLIENTE (Modal AJAX + SweetAlert)
// ================================
function abrirModalCrear() {
    $("#tituloModal").text("Registrar Nuevo Cliente");
    $("#contenidoModal").html(`
        <div class="text-center py-4">
            <div class="spinner-border text-primary" role="status">
                <span class="visually-hidden">Cargando...</span>
            </div>
        </div>`);
    mostrarModal();

    $.get("/Clientes/Create")
        .done(function (data) {
            $("#contenidoModal").html(data);

            $("#formNuevoCliente").off("submit").on("submit", function (e) {
                e.preventDefault();
                const form = $(this);

                $.ajax({
                    type: "POST",
                    url: form.attr("action"),
                    data: form.serialize(),
                    success: function (response) {
                        if (response.success) {
                            cerrarModal();
                            Swal.fire({
                                title: "✅ Cliente registrado",
                                text: "El cliente se agregó correctamente.",
                                icon: "success"
                            }).then(() => location.reload());
                        } else {
                            Swal.fire("Error", response.message || "No se pudo registrar el cliente.", "error");
                        }
                    },
                    error: function () {
                        Swal.fire("Error inesperado", "No se pudo procesar la solicitud.", "error");
                    }
                });
            });
        })
        .fail(() => $("#contenidoModal").html("<div class='text-danger text-center py-4'>❌ Error al cargar el formulario.</div>"));
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
    mostrarModal();

    $.get("/Clientes/Details/" + id)
        .done((data) => $("#contenidoModal").html(data))
        .fail(() => $("#contenidoModal").html("<div class='text-danger text-center py-4'>❌ Error al cargar los detalles.</div>"));
}

// ================================
// EDITAR CLIENTE
// ================================
function abrirModalEditar(id) {
    $("#tituloModal").text("Editar Cliente");
    $("#contenidoModal").html("<div class='text-center py-5 text-muted'>Cargando...</div>");
    mostrarModal();

    $.get("/Clientes/Edit/" + id)
        .done(function (data) {
            $("#contenidoModal").html(data);

            $("#formEditarCliente").off("submit").on("submit", function (e) {
                e.preventDefault();
                const form = $(this);

                $.ajax({
                    type: "POST",
                    url: form.attr("action"),
                    data: form.serialize(),
                    success: function (response) {
                        if (response.success) {
                            cerrarModal();
                            Swal.fire({
                                title: "Cliente actualizado",
                                text: "Los cambios se guardaron correctamente.",
                                icon: "success"
                            }).then(() => location.reload());
                        } else {
                            Swal.fire("Error", response.message || "No se pudo actualizar el cliente.", "error");
                        }
                    },
                    error: function () {
                        Swal.fire("Error inesperado", "No se pudo procesar la solicitud.", "error");
                    }
                });
            });
        })
        .fail(() => $("#contenidoModal").html("<div class='text-danger text-center py-4'>❌ Error al cargar los datos.</div>"));
}

// ================================
// ELIMINAR CLIENTE
// ================================
function abrirModalEliminar(id) {
    $("#tituloModal").html(`<i class="bi bi-trash3 text-danger"></i> Eliminar Cliente`);
    $("#contenidoModal").html(`
        <div class="d-flex flex-column align-items-center justify-content-center py-5 text-center text-muted">
            <div class="spinner-border text-danger mb-3" style="width: 3rem; height: 3rem;" role="status"></div>
            <p class="fw-semibold">Cargando información del cliente...</p>
        </div>
    `);
    mostrarModal();

    $.get("/Clientes/Delete/" + id)
        .done(function (data) {
            if (data && data.trim().length > 0) {
                $("#contenidoModal").hide().html(data).fadeIn(300);
            } else {
                mostrarErrorModal("No se pudo cargar el cliente seleccionado.");
            }
        })
        .fail((xhr) => mostrarErrorModal(xhr.responseText || "Error al obtener los datos del cliente."));
}

// 🎨 Mostrar error dentro del modal
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
