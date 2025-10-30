$(document).ready(function () {
    console.log("✅ Instructores.js cargado correctamente (modo Bootstrap 5)");
});

/* ============================
   🔧 Funciones auxiliares modal
============================ */
function mostrarModalInstructor() {
    const modalEl = document.getElementById("modalInstructor");
    const modal = bootstrap.Modal.getOrCreateInstance(modalEl);
    modal.show();
}

function cerrarModalInstructor() {
    const modalEl = document.getElementById("modalInstructor");
    const modal = bootstrap.Modal.getInstance(modalEl);
    if (modal) modal.hide();
}

/* ============================
   🧩 ABRIR MODAL CREAR
============================ */
function abrirModalCrearInstructor() {
    $("#tituloModal").text("Agregar Instructor");
    $("#contenidoModal").load("/Instructores/Create", function () {
        mostrarModalInstructor();
    });
}

/* ============================
   🧩 ABRIR MODAL EDITAR
============================ */
function abrirModalEditarInstructor(id) {
    $("#tituloModal").text("Editar Instructor");
    $("#contenidoModal").html("<div class='text-center py-4'>Cargando...</div>");

    $.get("/Instructores/Edit/" + id)
        .done(function (html) {
            $("#contenidoModal").html(html);
            mostrarModalInstructor();

            // ✅ Escuchar submit del formulario
            $("#formEditarInstructor").off("submit").on("submit", function (e) {
                e.preventDefault();
                const form = $(this);

                $.ajax({
                    type: "POST",
                    url: "/Instructores/Edit",
                    data: form.serialize(),
                    success: function (resp) {
                        if (resp.success) {
                            cerrarModalInstructor();
                            Swal.fire({
                                icon: "success",
                                title: "Instructor actualizado",
                                timer: 1500,
                                showConfirmButton: false
                            });
                            setTimeout(() => location.reload(), 1000);
                        } else {
                            Swal.fire("Error", resp.message || "No se pudo actualizar.", "error");
                        }
                    },
                    error: function () {
                        Swal.fire("Error", "Error inesperado al procesar la solicitud.", "error");
                    }
                });
            });
        })
        .fail(function () {
            $("#contenidoModal").html("<div class='text-danger text-center py-4'>❌ Error al cargar los datos.</div>");
        });
}

/* ============================
   🧩 ABRIR MODAL DETALLES
============================ */
function abrirModalDetallesInstructor(id) {
    $("#tituloModal").text("Detalles del Instructor");
    $("#contenidoModal").html("<div class='text-center py-4'>Cargando...</div>");
    $.get("/Instructores/Details/" + id)
        .done(function (data) {
            $("#contenidoModal").html(data);
            mostrarModalInstructor();
        })
        .fail(function () {
            $("#contenidoModal").html("<div class='text-danger text-center py-4'>❌ Error al cargar los detalles.</div>");
        });
}

/* ============================
   🧩 ABRIR MODAL ELIMINAR
============================ */
function abrirModalEliminarInstructor(id) {
    $("#tituloModal").text("Eliminar Instructor");
    $("#contenidoModal").html("<div class='text-center py-4'>Cargando...</div>");
    $.get("/Instructores/Delete/" + id)
        .done(function (data) {
            $("#contenidoModal").html(data);
            mostrarModalInstructor();
        })
        .fail(function () {
            $("#contenidoModal").html("<div class='text-danger text-center py-4'>❌ Error al cargar el instructor.</div>");
        });
}

/* ============================
   🧩 FORMULARIO ELIMINAR
============================ */
$(document).on("submit", "#formEliminarInstructor", function (e) {
    e.preventDefault();
    const data = $(this).serialize();

    $.post("/Instructores/DeleteConfirmed", data)
        .done(function (resp) {
            if (resp.success) {
                cerrarModalInstructor();
                Swal.fire({
                    icon: "success",
                    title: "Instructor eliminado correctamente",
                    timer: 1500,
                    showConfirmButton: false
                });
                setTimeout(() => location.reload(), 1000);
            } else {
                Swal.fire("Error", resp.message || "No se pudo eliminar el instructor.", "error");
            }
        })
        .fail(function () {
            Swal.fire("Error", "No se pudo procesar la solicitud.", "error");
        });
});
