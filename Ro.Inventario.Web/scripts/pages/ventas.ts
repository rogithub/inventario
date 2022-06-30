import { BinderService } from '../services/binderService';
import { Api } from '../services/api';

export interface IProduct
{
    nid: number;
    id: string;
    nombre: string;
    unidadMedida: string;
    categoria: string;
    codigoBarrasItem: string;
    codigoBarrasCaja: string;
    precioVenta: number;
}


export interface VentaProductoModel
{     
    productoId: string;
    cantidad: number;
}

export interface VentaModel
{
  pago: number;
  cambio: number;
  items: VentaProductoModel[];
}

export class ProductLine {
    public producto: IProduct;
    public cantidad: KnockoutObservable<number>;
    public total: KnockoutComputed<number>;

    constructor(product: IProduct)
    {
        this.producto = product;
        this.cantidad = ko.observable<number>(1);
        const self = this;
        this.total = ko.computed<number>(function() {
            return self.cantidad() * self.producto.precioVenta;
        });
    }
}

export class Venta {    
    public lines: KnockoutObservableArray<ProductLine>;
    public api: Api;
    public url: string;
    public autocomplete: Element;
    public total: KnockoutComputed<number>;
    public pagoCliente: KnockoutObservable<number>;
    public cambio: KnockoutComputed<number>;
    public isValid: KnockoutComputed<boolean>;

    constructor() {
        this.url = "ventas/Guardar"
        this.api = new Api();
        this.lines = ko.observableArray<ProductLine>([]);
        this.autocomplete = document.querySelector("#autoComplete");
        this.pagoCliente = ko.observable<number>(0);
        const self = this;        
        self.autocomplete.addEventListener("selection", function(e: any) {
            let p = e.detail.selection.value as IProduct;
            self.lines.push(new ProductLine(p));
            $(self.autocomplete).val("");
            return false;
        });

        this.total = ko.computed<number>(function() {
            let lines = self.lines();
            var initialValue = 0;
            return lines.reduce((sum, prod) => sum + prod.total(), initialValue);
        });        
        this.cambio = ko.computed<number>(function() {            
            return self.pagoCliente() - self.total();
        });

        this.isValid = ko.computed<boolean>(function() {            
            for (const l of self.lines())
            {
                if (l.cantidad() === 0 || isNaN(l.cantidad())) return false;
            }
            
            return !(isNaN(self.total()) || self.total() === 0 || 
                   isNaN(self.pagoCliente()) || self.pagoCliente() === 0 ||
                   self.total() > self.pagoCliente());
        });
    }

    public borrar(line: ProductLine): void {
        const self = this;
        self.lines.remove(line);
    }

    public bind(): void {
        const self = this;
        BinderService.bind(self, "#ventasPage");
    }


    public async guardar(): Promise<void> {
        const self = this;
        let lines = new Array<VentaProductoModel>();
        self.lines().forEach(l => {
            let line: VentaProductoModel =
            {
                cantidad: l.cantidad(),
                productoId: l.producto.id,            
            };
            lines.push(line);
        });
        let data: VentaModel = {
            cambio: self.cambio() as number,
            pago: self.pagoCliente() as number,
            items: lines
        };
        let url = `${self.url}`;

        let result = await self.api.post<number[]>(url, data);
        console.log(`Ventas guardadas ${result[0]} productos en esa venta ${result[1]}`);

        alert("Guardado!");
        window.location.href = `${document.baseURI}`;
    }
}

document.addEventListener('DOMContentLoaded', function () {
    var page = new Venta();
    page.bind();
    console.log("binding ko");    
}, false);