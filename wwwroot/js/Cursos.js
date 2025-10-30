$(document).ready(function () {
    console.log("✅ Cursos.js cargado correctamente");
});

/* ===========================
   ✅ AJAX - Crear Curso
=========================== */
function abrirModalCrearCurso() {
    $("#tituloModal").text("Agregar Curso");
    $("#contenidoModal").load("/Cursos/Create", function () {
        $("#modalCurso").modal("show");
    });
}

$(document).on("submit", "#formCrearCurso", function (e) {
    e.preventDefault();

    // ✅ Tomar el valor desde el campo visible y limpiarlo
    let costoVisual = $("#CostoVisual").val() || "0";
    let costoLimpio = costoVisual.replace(/[₡,. ]/g, ""); // elimina símbolos y comas
    $("#Costo").val(costoLimpio); // copiar al input hidden que se envía al backend

    // ✅ Serializar los datos y mostrarlos para depurar
    const data = $(this).serialize();
    console.log("📤 DATA A ENVIAR:", data);

    // ✅ Enviar por AJAX al controlador
    $.post("/Cursos/Create", data, function (resp) {
        console.log("🟢 RESPUESTA DEL SERVIDOR:", resp);
        mostrarResultado(resp, "Curso registrado correctamente.");
    }).fail(function (xhr, status, error) {
        console.error("❌ AJAX FAIL:", error);
        console.log("📦 DETALLES:", xhr.responseText);
        Swal.fire("Error", xhr.responseText || "Error al guardar el curso.", "error");
    });
});

/* ===========================
   ✅ AJAX - Editar Curso
=========================== */
function abrirModalEditar(id) {
    $("#tituloModal").text("Editar Curso");
    $("#contenidoModal").load("/Cursos/Edit/" + id, function () {
        $("#modalCurso").modal("show");
    });
}

$(document).on("submit", "#formEditarCurso", function (e) {
    e.preventDefault();

    // ✅ 1. Tomar el valor visible
    let costoVisual = $("#CostoVisual").val() || "0";

    // ✅ 2. Limpiar símbolos y convertir a número decimal
    let costoLimpio = costoVisual
        .replace(/[₡]/g, "")   // quita símbolo ₡
        .replace(/\s/g, "")    // quita espacios
        .replace(/\./g, "")    // quita puntos de miles
        .replace(",", ".");    // cambia coma decimal a punto

    // ✅ 3. Asegurar que haya un número válido
    if (isNaN(parseFloat(costoLimpio)) || parseFloat(costoLimpio) <= 0) {
        Swal.fire("Error", "Ingrese un costo válido mayor a 0.", "error");
        return;
    }

    // ✅ 4. Copiar valor limpio al hidden
    $("#Costo").val(costoLimpio);

    const data = $(this).serialize();
    console.log("📤 DATA A ENVIAR (Editar):", data);

    // ✅ 5. Enviar al controlador
    $.post("/Cursos/Edit", data, function (resp) {
        console.log("🟢 RESPUESTA DEL SERVIDOR:", resp);
        mostrarResultado(resp, "Curso actualizado correctamente.");
    }).fail(function (xhr) {
        Swal.fire("Error", xhr.responseText || "Error al actualizar el curso.", "error");
    });
});




/* ===========================
   ✅ AJAX - Detalles
=========================== */
function abrirModalDetalles(id) {
    $("#tituloModal").text("Detalles del Curso");
    $("#contenidoModal").load("/Cursos/Details/" + id);
    $("#modalCurso").modal("show");
}

/* ===========================
   ✅ AJAX - Eliminar
=========================== */
function abrirModalEliminar(id) {
    $("#tituloModal").text("Eliminar Curso");
    $("#contenidoModal").load("/Cursos/Delete/" + id);
    $("#modalCurso").modal("show");
}

$(document).on("submit", "#formEliminarCurso", function (e) {
    e.preventDefault();
    const data = $(this).serialize();

    $.post("/Cursos/DeleteConfirmed", data, function (resp) {
        console.log("🟢 RESPUESTA SERVIDOR (Eliminar):", resp);
        mostrarResultado(resp, "Curso eliminado correctamente.");
    }).fail(function (xhr, status, error) {
        console.error("❌ AJAX FAIL (Eliminar):", error);
        console.log("📦 DETALLES:", xhr.responseText);
        Swal.fire("Error", xhr.responseText || "Error al eliminar el curso.", "error");
    });
});

/* ===========================
   ✅ Manejador Modal
=========================== */
$(document).on("hidden.bs.modal", function () {
    $('body').removeAttr('aria-hidden');
});

/* ===========================
   ✅ SweetAlert2 - Mensajes
=========================== */
function mostrarResultado(resp, textoOK) {
    if (resp.success) {
        Swal.fire({
            icon: "success",
            title: textoOK,
            timer: 1500,
            showConfirmButton: false
        });
        $("#modalCurso").modal("hide");
        setTimeout(() => location.reload(), 1200);
    } else {
        Swal.fire({
            icon: "error",
            title: "Error",
            text: resp.message || "Error desconocido en servidor"
        });
    }
}
