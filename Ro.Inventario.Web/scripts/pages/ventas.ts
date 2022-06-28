import { BinderService } from '../services/binderService';
import { Api } from '../services/api';

export class Ventas {

    public pattern: KnockoutObservable<string>;
    public api: Api;
    public url: string;

    constructor() {
        this.url = $("#productSearch").val() as string;
        this.api = new Api();
        this.pattern = ko.observable<string>("");
    }

    public async load(): Promise<void> {
        const self = this;
    }

    public bind(): void {
        const self = this;
        BinderService.bind(self, "#ventasPage");
    }
}

document.addEventListener('DOMContentLoaded', function () {
    var page = new Ventas();
    page.bind();
    console.log("binding ko");
}, false);