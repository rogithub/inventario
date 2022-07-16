import { BinderService } from '../services/binderService';
import { Api } from '../services/api';
import toCurrency from '../shared/toCurrency';

export interface IProduct {
    nid: number;
    id: string;
    nombre: string;
    unidadMedida: string;
    categoria: string;
    codigoBarrasItem: string;
    codigoBarrasCaja: string;
    precioVenta: number;
    stock: number;
    precioVentaPesos: KnockoutComputed<string>;
}


export class Etiquetas {
    public lines: KnockoutObservableArray<IProduct>;
    public api: Api;
    public url: string;
    public autocomplete: Element;

    constructor() {
        this.url = "productos/descargar"
        this.api = new Api();
        this.lines = ko.observableArray<IProduct>([]);
        this.autocomplete = document.querySelector("#autoComplete");

        const self = this;
        self.autocomplete.addEventListener("selection", function (e: any) {
            let p = e.detail.selection.value as IProduct;
            p.precioVentaPesos = ko.computed<string>(() => toCurrency(p.precioVenta), p);
            self.lines.push(p);
            $(self.autocomplete).val("");
            return false;
        });
    }

    public borrar(p: IProduct): void {
        const self = this;
        self.lines.remove(p);
    }

    public bind(): void {
        const self = this;
        BinderService.bind(self, "#etiquetasPage");
    }
}

document.addEventListener('DOMContentLoaded', function () {
    var page = new Etiquetas();
    page.bind();
    console.log("binding ko");
}, false);