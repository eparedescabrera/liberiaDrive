// =====================================================
// 📦 Módulo: Vehículos (LiberiaDrive)
// Autor: José Paredes Cabrera
// =====================================================

// 🎯 Función genérica para abrir modal con carga
function abrirModalGenerico(titulo, url) {
    $("#tituloModal").html(titulo);
    $("#contenidoModal").html(`
        <div class="d-flex flex-column align-items-center justify-content-center py-5 text-center text-muted">
            <div class="spinner-border text-primary mb-3" style="width: 3rem; height: 3rem;" role="status"></div>
            <p class="fw-semibold">Cargando información...</p>
        </div>
    `);

    const modal = new bootstrap.Modal(document.getElementById("modalVehiculo"));
    modal.show();

    $.ajax({
        url: url,
        type: "GET",
        success: function (data) {
            $("#contenidoModal").hide().html(data).fadeIn(300);
        },
        error: function (xhr) {
            $("#contenidoModal").html(`
                <div class="text-center py-5">
                    <i class="bi bi-exclamation-triangle text-danger display-4"></i>
                    <p class="mt-3 fw-bold text-danger">Error al cargar el contenido.</p>
                    <p class="text-muted">${xhr.responseText}</p>
                </div>
            `);
        }
    });
}

// =====================================================
// 🆕 CREAR VEHÍCULO
// =====================================================
function abrirModalCrear() {
    abrirModalGenerico(`<i class="bi bi-plus-circle"></i> Nuevo Vehículo`, "/Vehiculos/Create");
}

// =====================================================
// ✏️ EDITAR VEHÍCULO
// =====================================================
function abrirModalEditar(id) {
    abrirModalGenerico(`<i class="bi bi-pencil-square"></i> Editar Vehículo`, `/Vehiculos/Edit/${id}`);
}

// =====================================================
// 🔍 DETALLES VEHÍCULO
// =====================================================
function abrirModalDetalles(id) {
    abrirModalGenerico(`<i class="bi bi-eye"></i> Detalles del Vehículo`, `/Vehiculos/Details/${id}`);
}

// =====================================================
// 🗑️ ELIMINAR VEHÍCULO
// =====================================================
function abrirModalEliminar(id) {
    abrirModalGenerico(`<i class="bi bi-trash3 text-danger"></i> Eliminar Vehículo`, `/Vehiculos/Delete/${id}`);
}

// =====================================================
// 💾 EVENTO SUBMIT GLOBAL para Crear/Editar
// =====================================================
$(document).off("submit", "#formCrearVehiculo, #formEditarVehiculo")
.on("submit", "#formCrearVehiculo, #formEditarVehiculo", function (e) {
    e.preventDefault();

    const form = $(this);
    const action = form.attr("id").includes("Editar") ? "actualizado" : "registrado";

    $.ajax({
        url: form.attr("id").includes("Editar") ? "/Vehiculos/Edit" : "/Vehiculos/Create",
        type: "POST",
        data: form.serialize(),
        success: function (resp) {
            if (resp.success) {
                Swal.fire({
                    icon: "success",
                    title: `Vehículo ${action} correctamente`,
                    showConfirmButton: false,
                    timer: 1500
                });
                $("#modalVehiculo").modal("hide");
                setTimeout(() => location.reload(), 1200);
            } else {
                Swal.fire("Error", resp.message, "error");
            }
        },
        error: function () {
            Swal.fire("Error", "No se pudo procesar la solicitud.", "error");
        }
    });
});

// =====================================================
// 🧹 EVENTO SUBMIT para ELIMINAR
// =====================================================
$(document).off("submit", "#formEliminarVehiculo")
.on("submit", "#formEliminarVehiculo", function (e) {
    e.preventDefault();
    const id = $("#IdVehiculo").val();

    Swal.fire({
        title: "¿Eliminar vehículo?",
        text: "Esta acción no se puede deshacer.",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#d33",
        cancelButtonColor: "#6c757d",
        confirmButtonText: "Sí, eliminar",
        cancelButtonText: "Cancelar"
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: "/Vehiculos/DeleteConfirmed",
                type: "POST",
                data: { IdVehiculo: id },
                success: function (resp) {
                    if (resp.success) {
                        Swal.fire({
                            icon: "success",
                            title: "Vehículo eliminado",
                            text: resp.message,
                            timer: 1500,
                            showConfirmButton: false
                        });
                        $("#modalVehiculo").modal("hide");
                        setTimeout(() => location.reload(), 1200);
                    } else {
                        Swal.fire("Error", resp.message, "error");
                    }
                },
                error: function () {
                    Swal.fire("Error", "No se pudo eliminar el vehículo.", "error");
                }
            });
        }
    });
});
