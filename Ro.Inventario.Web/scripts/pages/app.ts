import { BinderService } from '../services/binderService';
import { Api } from '../services/api';

export class App {

    public title: KnockoutObservable<string>;
    public api: Api;
    public url: string;

    constructor() {

        this.url = "api/Feeds/client";
        this.api = new Api();
        this.title = ko.observable<string>("Titulo de prueba desde Knockout!!!");
    }

    public async load(): Promise<void> {
        const self = this;
    }

    public bind(): void {
        const self = this;
        BinderService.bind(self, "#roApp");
    }
}

document.addEventListener('DOMContentLoaded', function () {
    var page = new App();
    page.bind();
    console.log("binding ko");
}, false);