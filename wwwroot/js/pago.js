// =====================================================
// 💳 GESTIÓN DE PAGOS - CRUD AJAX + SWEETALERT + MODAL
// =====================================================

// =====================================================
// 🟢 CREAR PAGO
// =====================================================
function abrirModalCrearPago() {
    $("#tituloModal").text("Agregar Pago");
    $("#contenidoModal").load("/Pago/Create", function () {
        $("#modalPago").modal("show");

        // Inicializar Select2 dentro del modal
        inicializarSelectInscripcion();

        // Vincular formulario
        vincularFormularioPago("#formCrearPago", "/Pago/Create", "Pago registrado correctamente");
    });
}

// =====================================================
// ✏️ EDITAR PAGO
// =====================================================
function abrirModalEditarPago(id) {
    $("#tituloModal").text("Editar Pago");
    $("#contenidoModal").load("/Pago/Edit/" + id, function () {
        $("#modalPago").modal("show");

        // Inicializar Select2 después de cargar el modal
        inicializarSelectInscripcion();

        // Vincular evento del formulario de edición
        vincularFormularioPago("#formEditarPago", "/Pago/Edit", "Pago actualizado correctamente");
    });
}

// =====================================================
// ❌ ELIMINAR PAGO
// =====================================================
function abrirModalEliminarPago(id) {
    $("#tituloModal").text("Eliminar Pago");
    $("#contenidoModal").load("/Pago/Delete/" + id, function () {
        $("#modalPago").modal("show");

        $("#formEliminarPago").on("submit", function (e) {
            e.preventDefault();

            $.post("/Pago/DeleteConfirmed", $(this).serialize(), function (resp) {
                if (resp.success) {
                    mostrarResultado("Pago eliminado correctamente");
                } else {
                    mostrarError(resp.message || "Error al eliminar el pago.");
                }
            });
        });
    });
}

// =====================================================
// 🔄 FUNCIÓN REUTILIZABLE PARA CREAR Y EDITAR PAGOS
// =====================================================
function vincularFormularioPago(formSelector, url, mensajeExito) {
    $(formSelector).on("submit", function (e) {
        e.preventDefault();

        const data = $(this).serialize();

        $.post(url, data, function (resp) {
            if (resp.success) {
                mostrarResultado(mensajeExito);
            } else {
                mostrarError(resp.message || "Ocurrió un error al procesar el pago.");
            }
        });
    });
}

// =====================================================
// 🧭 FUNCIÓN GLOBAL: SELECT2 INSCRIPCIONES + AUTOLLENADO
// =====================================================
function inicializarSelectInscripcion() {
    $("#IdInscripcion").select2({
        theme: "bootstrap-5",
        placeholder: "Buscar inscripción (cliente o curso)...",
        dropdownParent: $("#modalPago"),
        ajax: {
            url: "/Pago/BuscarInscripciones",
            dataType: "json",
            delay: 250,
            data: function (params) {
                return { Texto: params.term };
            },
            processResults: function (data) {
                return {
                    results: data.map(item => ({
                        id: item.id,
                        text: item.text,
                        costo: item.costo
                    }))
                };
            },
            cache: true
        }
    });

    // 💰 Auto llenar monto al seleccionar una inscripción
    $("#IdInscripcion").on("select2:select", function (e) {
        const data = e.params.data;
        console.log("➡️ Inscripción seleccionada:", data);
        if (data && data.costo) {
            $("#Monto").val(data.costo.toFixed(2));
        }
    });

    // 🔄 Limpiar select al cerrar modal
    $('#modalPago').on('hidden.bs.modal', function () {
        $("#IdInscripcion").val(null).trigger('change');
    });
}

// =====================================================
// ✅ MOSTRAR RESULTADO EXITOSO (SweetAlert global)
// =====================================================
function mostrarResultado(mensaje) {
    $("#modalPago").modal("hide");

    Swal.fire({
        icon: 'success',
        title: mensaje,
        showConfirmButton: false,
        timer: 1500
    }).then(() => location.reload());
}

// =====================================================
// ⚠️ MOSTRAR ERROR (SweetAlert global)
// =====================================================
function mostrarError(mensaje) {
    Swal.fire({
        icon: 'error',
        title: 'Error',
        text: mensaje || "Ocurrió un error inesperado.",
        confirmButtonText: 'Aceptar'
    });
}
