import { BinderService } from '../services/binderService';
import { Api } from '../services/api';

export interface IProduct
{
    id: string,
    value: string
}

export class Ventas {

    public pattern: KnockoutObservable<string>;
    public productos: KnockoutObservableArray<IProduct>;
    public api: Api;
    public url: string;

    constructor() {
        this.url = "ventas/buscarProducto"
        this.api = new Api();
        this.pattern = ko.observable<string>("");
        this.productos = ko.observableArray<IProduct>([]);
        const self = this;
        this.pattern.subscribe(async (newValue) => {
            let apiUrl = `${self.url}?pattern=${newValue}`;
            let prods = await self.api.get<IProduct[]>(apiUrl);
            self.productos.removeAll();
            prods.forEach(element => {
                console.log(element);
                self.productos.push(element);
            });
            
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