function roleEditorInitialize(container, options) {
    $('<input required data-text-field="Name" data-value-field="Id" data-bind="value:' + options.field + '"/>')
        .appendTo(container)
        .kendoDropDownList({
            autoBind: false,
            dataSource: {
                transport: {
                    read: "/api/Users/GetUserRoles"
                }
            }
        });
}

function customEditor(container, options) {
    var element = $("<textarea/>");
    element.attr("name", options.field);
    element.attr("id", options.field);
    element.attr("style", "width: 100%");
    element.appendTo(container);
    element.kendoEditor({
    });
}

var talesFormInit = function (divId) {

   
    $("#" + divId + "Grid").kendoGrid({
        toolbar: ["create"],
        pageable: true,
        sortable: true,
        editable: {
            mode: "popup",
            title: "Редагувати",
            window: { "width": 982 },
            createAt: "top",
        },
        edit: function (e) {
            $("#Text").data("kendoEditor").wrapper.width("100%").height("").addClass("expandEditor");
        },
        columns: [
            {
                field: "Name",
                title: "Назва",
                width: "200px",
                attributes: {
                    style: "text-align: center;"
                }
            },  {
                field: "Description",
                title: "Опис",
                width: "300px",
                attributes: {
                    style: "text-align: center;"
                }
            },{
                field: "Text",
                title: "Текст",
                editor: customEditor,
                height: "200px",
                template: "<div style='height:200px;overflow:scroll'>#: Text #</div>"
            },

            { command: ["edit", "destroy"], title: "&nbsp;", width: "250px" }],
        dataSource: {
            transport: {
                read: {
                    url: "/api/Tales",
                    contentType: "application/json"
                },
                create: {
                    url: "/api/Tales/",
                    contentType: "application/json",
                    type: "POST"
                },
                update: {
                    url: function (obj) {
                        return "/api/Tales/PutTale/" + obj.Id;
                    },
                    contentType: "application/json",
                    type: "PUT"
                },
                destroy: {
                    url: function (obj) {
                        return "/api/Tales/DeleteTale/" + obj.Id;
                    },
                    contentType: "application/json",
                    type: "DELETE"
                },
                parameterMap: function (data, operation) {
                    return JSON.stringify(data);
                }
            },
            schema: {
                model: {
                    id: "Id",
                    fields: {
                        Id: { from: "Id", type: "number", defaultValue: 0 },
                        Name: { from: "Name", validation: { required: true } },
                        Description: { from: "Description", validation: { required: true } },
                        Text: { from: "Text", validation: { required: true } }
                    }
                }
            },
            pageSize: 10
        }
    });
}