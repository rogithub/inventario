import { BinderService } from '../services/binderService';
import { Api } from '../services/api';

export class Ventas {

    public pattern: KnockoutObservable<string>;
    public api: Api;
    public url: string;

    constructor() {
        this.url = "ventas/buscarProducto"
        //$("#productSearch").val() as string;
        this.api = new Api();
        this.pattern = ko.observable<string>("");
        const self = this;
        this.pattern.subscribe(async (newValue) => {
            let apiUrl = `${self.url}?pattern=${newValue}`;
            let prods = await self.api.get(apiUrl);
            console.log(prods);
        });
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