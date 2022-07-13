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


export class Productos {
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
            p.precioVentaPesos = ko.computed<string>(()=> toCurrency(p.precioVenta) , p);
            self.lines.push(p);
            $(self.autocomplete).val("");
            return false;
        });        
    }    

    public borrar(p: IProduct): void {
        const self = this;
        self.lines.remove(p);
    }
    
    public download(): void {
        const self = this;
        let csv = 
        "ID,NOMBRE,CANTIDAD,PRECIO COMPRA,PRECIO VENTA,CODIGO BARRAS ITEM,CODIGO BARRAS CAJA,UNIDAD MEDIDA,CATEGORIA\n";
        for(let it of self.lines())
        {
            var row = `${it.id},${it.nombre},,,${it.precioVenta},${it.codigoBarrasItem},${it.codigoBarrasCaja},${it.unidadMedida},${it.categoria}\n`;
            csv += row;
        }
       
        var hiddenElement = document.createElement('a');  
        hiddenElement.href = 'data:text/csv;charset=utf-8,' + encodeURI(csv);  
        hiddenElement.target = '_blank';  
        
        //provide the name for the CSV file to be downloaded  
        hiddenElement.download = 'inventario_ProductosExistentes.csv';  
        hiddenElement.click();  
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