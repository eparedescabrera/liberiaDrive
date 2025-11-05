$(document).ready(function () {

    // Inicializar DataTable
    $('#tablaHorarios').DataTable({
        language: { url: '//cdn.datatables.net/plug-ins/1.13.6/i18n/es-ES.json' },
        pageLength: 10
    });

    // Crear
    window.abrirModalCrearHorario = function () {
        $("#tituloModal").text("Registrar Horario de Instructor");
        $("#contenidoModal").load("/HorarioInstructor/Create", function () {
            $("#modalHorario").modal("show");
            inicializarFormulario();
        });
    };

    // Editar
    window.abrirModalEditar = function (id) {
        $("#tituloModal").text("Editar Horario");
        $("#contenidoModal").load("/HorarioInstructor/Edit/" + id, function () {
            $("#modalHorario").modal("show");
            inicializarFormulario();
        });
    };

    // Eliminar
    window.abrirModalEliminar = function (id) {
        Swal.fire({
            title: "¿Eliminar horario?",
            text: "Esta acción no se puede deshacer.",
            icon: "warning",
            showCancelButton: true,
            confirmButtonText: "Sí, eliminar",
            cancelButtonText: "Cancelar"
        }).then(result => {
            if (result.isConfirmed) {
                $.post("/HorarioInstructor/DeleteConfirmed/" + id, function (resp) {
                    if (resp.success) {
                        Swal.fire("Eliminado", "El horario fue eliminado correctamente", "success")
                            .then(() => location.reload());
                    } else {
                        Swal.fire("Error", resp.message, "error");
                    }
                });
            }
        });
    };

    // Inicializar formulario
    function inicializarFormulario() {
        // Select2 instructores
        $('#IdInstructor').select2({
            dropdownParent: $('#modalHorario'),
            theme: 'bootstrap-5',
            width: '100%',
            placeholder: 'Seleccione un instructor',
            allowClear: true,
            ajax: {
                url: '/HorarioInstructor/BuscarInstructores',
                dataType: 'json',
                delay: 250,
                data: params => ({ term: params.term }),
                processResults: data => ({ results: data })
            }
        });

        // Crear
        $(document).off('submit', '#formCrearHorario').on('submit', '#formCrearHorario', function (e) {
            e.preventDefault();
            $.post('/HorarioInstructor/Create', $(this).serialize())
                .done(resp => {
                    if (resp.success)
                        Swal.fire("Éxito", "Horario registrado correctamente", "success").then(() => location.reload());
                    else
                        Swal.fire("Advertencia", resp.message, "warning");
                });
        });

        // Editar
        $(document).off('submit', '#formEditarHorario').on('submit', '#formEditarHorario', function (e) {
            e.preventDefault();
            $.post('/HorarioInstructor/Edit', $(this).serialize())
                .done(resp => {
                    if (resp.success)
                        Swal.fire("Éxito", "Horario actualizado correctamente", "success").then(() => location.reload());
                    else
                        Swal.fire("Advertencia", resp.message, "warning");
                });
        });
    }
});

