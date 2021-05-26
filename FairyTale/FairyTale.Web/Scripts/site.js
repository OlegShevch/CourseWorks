
var talesFormInit = function (divId) {
       
    var dataSource = new kendo.data.DataSource({
        transport: {
            read: {
                url: "/api/Tales/GetDetails/None",
                contentType: "application/json"
            },
            parameterMap: function (data, operation) {
                return JSON.stringify(data);
            }
        },
        batch: true,
        pageSize: 20,
        autoSync: true,
        schema: {
            model: {
                id: "Id",
                fields: {
                        Id: { editable: false, nullable: true },
                        Name: { editable: false },
                        Rating: { editable: false },
                        Status: { editable: false },
                        Description: { editable: false },
                        Favorite: { editable: false },
                        CreatedOn: { editable: false, type: "date" },
                    }
                }
            }
        });

    $("#" + divId).kendoGrid({
        dataSource: dataSource,
        columnMenu: {
            filterable: false
        },
        height: 680,
        pageable: true,
        sortable: true,
        navigatable: true,
        resizable: true,
        reorderable: true,
        filterable: true,
        dataBound: onDataBound,
        toolbar: ["excel", "pdf",],
        columns: [{
            field: "Name",
            title: "Назва",
            template: '<a href="#= "/Home/TaleView?Id=" + Id #">#= Name #</a>',
            width: 200
        }, {
            field: "Description",
            title: "Опис",
            width: 300,
        }, {
            field: "Rating",
            title: "Рейтинг",
            template: "<input id='rating_#=Id#' data-bind='value: Rating' class='rating'/>",
            width: 200
         },{
            field: "Status",
            title: "Прочитанний",
            attributes: {
                style: "text-align: center;"
            },
            template: '<input type="checkbox" #= Status ? \'checked="checked"\' : "" # disabled="disabled" class="chkbx k-checkbox" />',
            width: 170
         },{
            field: "Favorite",
            title: "Улюблений",
            attributes: {
                style: "text-align: center;"
            },
            template: '<input id="favorite_#=Id#" type="checkbox" #= Favorite ? \'checked="checked"\' : "" # class="favorite k-checkbox" />',
            width: 170
        },{
            field: "CreatedOn",
            title: "Дата",
            format: "{0: yyyy-MM-dd HH:mm:ss}",
            width: 200
        }],
    });

    function onDataBound(e) {
        var grid = this;
        grid.table.find("tr").each(function () {
            var dataItem = grid.dataItem(this);

            $(this).find(".rating").kendoRating({
                min: 1,
                max: 5,
                label: false,
                selection: "continuous",
                change: onChange,
            });

            $(this).find(".favorite").click(function (e) {
                
                var id = $(e.currentTarget).attr("id").replace('favorite_', '');
                var isFavorite = $(this).is(':checked');

                $.ajax({
                    type: "post",
                    url: "/api/Tales/SetFavorite/" + id,
                    async: false,
                    dataType: 'json',
                    contentType: 'application/json; charset=utf-8',

                    success: function () {

                    },
                    error: function (xhr, status, error) {
                    }
                });

            });


            function onChange(e) {
                
                var id = $("input:first", $(e.target[0]).parent().parent()).attr('id').replace('rating_', '');
                
                $.ajax({
                    type: "post",
                    url: "/api/Tales/SetRating/" + id,
                    async: false,
                    dataType: 'json',
                    contentType: 'application/json; charset=utf-8',
                    data: JSON.stringify(e.newValue),

                    success: function () {

                    },
                    error: function (xhr, status, error) {
                    }
                });
                e.sender.enable(false);
            }

            kendo.bind($(this), dataItem);
        });
    }
};