var GridViewModel = function () {
    var self = this;

    ////////////////////////////////////////////////////////////////
    /// Constants
    ////////////////////////////////////////////////////////////////

    self.apiUrl = '/api/LogEntries/';

    ////////////////////////////////////////////////////////////////
    /// Observables
    ////////////////////////////////////////////////////////////////

    self.items = ko.observableArray([]);
    self.searchString = ko.observable("");
    self.currentPage = ko.observable("1");
    self.pageCount = ko.observable("1");
    self.currentLogFolder = ko.observable("");
    self.isProcessing = ko.observable(false);

    ////////////////////////////////////////////////////////////////
    /// API Calls
    ////////////////////////////////////////////////////////////////

    self.loadData = function (reload) {
        if (self.currentLogFolder() != "") {
            self.isProcessing(true);
            $.get(self.apiUrl,
                    {
                        CurrentLogFolder: self.currentLogFolder(),
                        SearchTerm: self.searchString(),
                        Page: self.currentPage(),
                        Reload: reload != null ? reload : false
                    }
                    , function (data) {
                        self.items(data.Items);
                        self.pageCount(data.PageCount);
                        self.isProcessing(false);
                    });
        }
    };
    self.loadData();



    ////////////////////////////////////////////////////////////////
    /// UI Triggers
    ////////////////////////////////////////////////////////////////

    self.reloadData = function () {
        self.loadData(true);
    };

    self.currentLogFolder.subscribe(function () {
        self.loadData(true);
    });

    self.previousPage = function () {
        if (self.currentPage() > 0) {
            self.currentPage(parseInt(self.currentPage()) - 1);
            self.loadData(false);
        }
    };

    self.nextPage = function () {
        if (self.currentPage() < self.pageCount()) {
            self.currentPage(parseInt(self.currentPage()) + 1);
            self.loadData(false);
        }
    };

    self.viewLogDetail = function (logEntry) {

        ko.mapping.fromJS(logEntry, {}, self.DetailViewModel);
        self.DetailViewModel.serverVariables(mapDictionaryToArray(logEntry.ServerVariables));
        self.DetailViewModel.queryStrings(mapDictionaryToArray(logEntry.QueryStrings));
        self.DetailViewModel.cookies(mapDictionaryToArray(logEntry.Cookies));

        $('#logDetailPopup').modal('show');
    };

    ////////////////////////////////////////////////////////////////
    /// Child ViewModels
    ////////////////////////////////////////////////////////////////

    var detailViewModel = function () {
        var detailSelf = this;

        detailSelf.ErrorType = ko.observable();
        detailSelf.ErrorMessage = ko.observable();
        detailSelf.ErrorDetail = ko.observable();
        detailSelf.Url = ko.observable();
        detailSelf.LogEntries = ko.observableArray();
        detailSelf.ServerVariables = ko.observableArray();
        detailSelf.QueryStrings = ko.observableArray();
        detailSelf.Cookies = ko.observableArray();

        detailSelf.serverVariables = ko.observableArray();
        detailSelf.queryStrings = ko.observableArray();
        detailSelf.cookies = ko.observableArray();

        detailSelf.clearLog = function (logEntry) {
            self.isProcessing(true);
            $.ajax({ type: 'DELETE', url: self.apiUrl + logEntry.LastErrorID() }).done(function (data) {
                $('#logDetailPopup').modal('hide');
                self.loadData();
                self.isProcessing(false);
            });
        };
    };
    self.DetailViewModel = new detailViewModel();
};