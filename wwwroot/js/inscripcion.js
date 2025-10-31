// ============================================================
// 📘 Módulo: InscripciónCurso
// Funcionalidad: CRUD AJAX + SweetAlert + Modal dinámico
// ============================================================

// ✅ Función global para mostrar alertas
function mostrarResultado(resp, mensajeExito) {
    if (resp.success) {
        Swal.fire({
            icon: "success",
            title: mensajeExito,
            showConfirmButton: false,
            timer: 2000
        }).then(() => location.reload());
    } else {
        Swal.fire({
            icon: "error",
            title: "Error",
            text: resp.message
        });
    }
}

// ============================================================
// 🟢 Abrir MODALES (Crear, Editar, Eliminar)
// ============================================================

function abrirModalCrear() {
    $("#tituloModal").text("Registrar Inscripción");
    $("#contenidoModal").load("/InscripcionCurso/Create", function () {
        $("#modalInscripcion").modal("show");
        inicializarSelect2(); // 👈 importante
    });
}



function abrirModalEditar(id) {
    $("#tituloModal").text("Editar Inscripción");
    $("#contenidoModal").load("/InscripcionCurso/Edit/" + id, function () {
        $("#modalInscripcion").modal("show");
    });
}

function abrirModalEliminar(id) {
    $("#tituloModal").text("Eliminar Inscripción");
    $("#contenidoModal").load("/InscripcionCurso/Delete/" + id, function () {
        $("#modalInscripcion").modal("show");
    });
}

// ============================================================
// 🟣 Inicializar Select2 dinámico (Clientes / Cursos)
// ============================================================

function inicializarSelect2() {
    $(".select2").select2({
        dropdownParent: $("#modalInscripcion"),
        width: "100%",
        placeholder: "Seleccione una opción",
        allowClear: true
    });

    // 🔍 Clientes
    $("#IdCliente").select2({
        ajax: {
            url: "/InscripcionCurso/BuscarClientes",
            dataType: "json",
            delay: 250,
            data: params => ({ term: params.term }),
            processResults: data => ({ results: data })
        }
    });

    // 🔍 Cursos
    $("#IdCurso").select2({
        ajax: {
            url: "/InscripcionCurso/BuscarCursos",
            dataType: "json",
            delay: 250,
            data: params => ({ term: params.term }),
            processResults: data => ({ results: data })
        }
    });
}

// ============================================================
// 🟠 Envíos AJAX (Create / Edit / Delete)
// ============================================================

// 🟢 Crear Inscripción
$(document).on("submit", "#formCrearInscripcion", function (e) {
    e.preventDefault();
    const data = $(this).serialize();

    $.post("/InscripcionCurso/Create", data, function (resp) {
        mostrarResultado(resp, "Inscripción registrada correctamente.");
    });
});

// 🟡 Editar Inscripción
$(document).on("submit", "#formEditarInscripcion", function (e) {
    e.preventDefault();
    const data = $(this).serialize();

    $.post("/InscripcionCurso/Edit", data, function (resp) {
        mostrarResultado(resp, "Inscripción actualizada correctamente.");
    });
});

// 🔴 Eliminar Inscripción
$(document).on("submit", "#formEliminarInscripcion", function (e) {
    e.preventDefault();
    const data = $(this).serialize();

    $.post("/InscripcionCurso/DeleteConfirmed", data, function (resp) {
        mostrarResultado(resp, "Inscripción eliminada correctamente.");
    });
});

// ============================================================
// ⚙️ Efectos visuales opcionales
// ============================================================

// Efecto hover suave para tarjetas (Index con cards)
$(document).on("mouseenter", ".card", function () {
    $(this).addClass("shadow-lg").css("transform", "translateY(-5px)");
});

$(document).on("mouseleave", ".card", function () {
    $(this).removeClass("shadow-lg").css("transform", "translateY(0)");
});
