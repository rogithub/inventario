import { BinderService } from '../services/binderService';
import { Api } from '../services/api';

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
}


export class Productos {
    public lines: KnockoutObservableArray<IProduct>;
    public api: Api;
    public url: string;
    public autocomplete: Element;                

    constructor() {        
        this.url = "productos/editar"
        this.api = new Api();
        this.lines = ko.observableArray<IProduct>([]);
        this.autocomplete = document.querySelector("#autoComplete");
        
        const self = this;
        self.autocomplete.addEventListener("selection", function (e: any) {
            let p = e.detail.selection.value as IProduct;
            self.lines.push(p);
            $(self.autocomplete).val("");
            return false;
        });        
    }   

    public editar(line: IProduct): void {
        const self = this;
        alert("Editando!");
    }

    public bind(): void {
        const self = this;
        BinderService.bind(self, "#productosPage");
    }    
}

document.addEventListener('DOMContentLoaded', function () {
    var page = new Productos();
    page.bind();
    console.log("binding ko");
}, false);