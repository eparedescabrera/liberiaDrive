$(document).ready(function () {
    console.log("✅ citas.js cargado correctamente");
});

/* ============================================================
   🧩 ABRIR MODAL CREAR CITA
============================================================ */
function abrirModalCrearCita() {
    $("#tituloModal").text("Registrar Nueva Cita");
    $("#contenidoModal").html(`
        <div class='text-center py-4'>
            <div class='spinner-border text-primary' role='status'>
                <span class='visually-hidden'>Cargando...</span>
            </div>
        </div>
    `);

    $("#modalCita").modal("show");

    // Cargar el formulario parcial
    $.get("/Citas/Create")
        .done(function (html) {
            $("#contenidoModal").html(html);

            // ✅ Inicializar Select2 dinámico dentro del modal
            $("#clienteSelect").select2({
                theme: "bootstrap-5",
                placeholder: "Seleccione un cliente...",
                allowClear: true,
                dropdownParent: $('#modalCita'), // 👈 evita error de escritura dentro del modal
                ajax: {
                    url: "/Citas/BuscarClientes",
                    dataType: "json",
                    delay: 250,
                    data: function (params) {
                        return { term: params.term };
                    },
                    processResults: function (data) {
                        return { results: data };
                    },
                    cache: true
                },
                language: {
                    inputTooShort: () => "Escriba al menos 1 carácter...",
                    searching: () => "Buscando clientes...",
                    noResults: () => "No se encontraron coincidencias"
                }
            });

            // ✅ Manejar envío del formulario con AJAX
            $("#formCrearCita").off("submit").on("submit", function (e) {
                e.preventDefault();
                const data = $(this).serialize();

                $.post("/Citas/Create", data)
                    .done(resp => {
                        if (resp.success) {
                            $("#modalCita").modal("hide");
                            Swal.fire({
                                icon: "success",
                                title: "Cita registrada",
                                text: resp.message,
                                timer: 2000,
                                showConfirmButton: false
                            }).then(() => location.reload());
                        } else {
                            Swal.fire("Error", resp.message, "error");
                        }
                    })
                    .fail(() => {
                        Swal.fire("Error", "Error al registrar la cita.", "error");
                    });
            });
        })
        .fail(function () {
            $("#contenidoModal").html(
                "<div class='text-danger text-center py-4'>❌ Error al cargar el formulario.</div>"
            );
        });
}

/* ============================================================
   🧩 ABRIR MODAL EDITAR CITA
============================================================ */
function abrirModalEditarCita(id) {
    $("#tituloModal").text("Editar Cita");
    $("#contenidoModal").html(`
        <div class='text-center py-5'>
            <div class='spinner-border text-primary'></div>
        </div>
    `);

    const modal = new bootstrap.Modal(document.getElementById("modalCita"));
    modal.show();

    $.get("/Citas/Edit/" + id)
        .done(function (data) {
            $("#contenidoModal").html(data);

            // ✅ Reaplicar Select2 dentro del modal (por si hay campo cliente)
            $("#clienteSelect").select2({
                theme: "bootstrap-5",
                dropdownParent: $('#modalCita'),
                placeholder: "Seleccione un cliente...",
                allowClear: true,
                ajax: {
                    url: "/Citas/BuscarClientes",
                    dataType: "json",
                    delay: 250,
                    data: params => ({ term: params.term }),
                    processResults: data => ({ results: data })
                }
            });
        })
        .fail(function () {
            $("#contenidoModal").html(
                "<div class='text-danger text-center py-4'>❌ Error al cargar los datos.</div>"
            );
        });
}

/* ============================================================
   🧩 ABRIR MODAL ELIMINAR CITA
============================================================ */
function abrirModalEliminarCita(id) {
    $("#tituloModal").text("Eliminar Cita");
    $("#contenidoModal").html(`
        <div class='text-center py-5'>
            <div class='spinner-border text-danger'></div>
        </div>
    `);

    const modal = new bootstrap.Modal(document.getElementById("modalCita"));
    modal.show();

    $.get("/Citas/Delete/" + id)
        .done(data => $("#contenidoModal").html(data))
        .fail(() =>
            $("#contenidoModal").html(
                "<div class='text-danger text-center py-4'>Error al cargar los datos.</div>"
            )
        );
}

/* ============================================================
   🧩 FORMULARIO ELIMINAR (SweetAlert)
============================================================ */
$(document).on("submit", "#formEliminarCita", function (e) {
    e.preventDefault();
    const data = $(this).serialize();

    $.post("/Citas/DeleteConfirmed", data)
        .done(resp => {
            if (resp.success) {
                $("#modalCita").modal("hide");
                Swal.fire({
                    icon: "success",
                    title: "Cita eliminada correctamente",
                    timer: 1500,
                    showConfirmButton: false
                });
                setTimeout(() => location.reload(), 1000);
            } else {
                Swal.fire("Error", resp.message || "No se pudo eliminar la cita.", "error");
            }
        })
        .fail(() => {
            Swal.fire("Error", "No se pudo procesar la solicitud.", "error");
        });
});
