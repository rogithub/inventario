import { BinderService } from '../services/binderService';
import { Api } from '../services/api';
import { parse } from 'path';

let getValueOrZero = (it: any) =>
{
    let n: number = isNaN(it) ? 0 : parseFloat(it) as number;
    return n;
}

export interface IAjuste {
    id: string;
    fechaAjuste: string;
    pago: number;
    cambio: number;
    tipoAjuste: number;
    iva: number;
}

export interface IProducto {
    id: string;
    unidadMedidaId: string;
    Nombre: string;
    codigoBarrasCaja: number;
    codigoBarrasItem: string;
}

interface IProductLine {
    producto: IProducto;
    ajusteProductoId: string;
    cantidad: number;
    precioUnitario: number;
    categoria: string;
    unidadMedida: string;
    cantidadEnBuenasCondiciones: number;
    cantidadEnMalasCondiciones: number;
}

interface model {
    venta: IAjuste,
    devueltos: IProductLine[]
}

class ProductLine {
    producto: IProducto;
    cantidadEnBuenasCondiciones: KnockoutObservable<number>;
    cantidadEnMalasCondiciones: KnockoutObservable<number>;
    ajusteProductoId: string;
    cantidad: number;
    precioUnitario: number;
    categoria: string;
    unidadMedida: string;
    devolucionRowOp: KnockoutComputed<number>;
    aMoneda: Function;
    constructor(l: IProductLine) {
        this.producto = l.producto;
        this.cantidadEnBuenasCondiciones = ko.observable<number>(0);
        this.cantidadEnMalasCondiciones = ko.observable<number>(0);
        this.ajusteProductoId = l.ajusteProductoId;
        this.cantidad = l.cantidad;
        this.precioUnitario = l.precioUnitario;
        this.categoria = l.categoria;
        this.unidadMedida = l.unidadMedida;
        const self = this;
        this.devolucionRowOp = ko.computed<number>(() => {
            let a = this.cantidadEnBuenasCondiciones();
            let b = this.cantidadEnMalasCondiciones();  
            let c = this.precioUnitario;
            //return ((a+b) * c);
            return (parseFloat(a.toString()) + parseFloat(b.toString())) * c;
        }, self);

        this.aMoneda = new Intl.NumberFormat('es-MX', {
            style: "currency",
            currency: "USD",
            currencyDisplay: "narrowSymbol"
        }).format;
    }
}

export class Devolucion {
    public api: Api;
    public url: string;
    public lines: KnockoutObservableArray<ProductLine>;
    public venta: KnockoutObservable<IAjuste>;
    constructor() {
        this.url = "ventas/GetVentaData"
        this.api = new Api();
        this.lines = ko.observableArray<ProductLine>();
        this.venta = ko.observable<IAjuste>();
    }

    public async load(): Promise<void> {
        const self = this;
        const ventaId = $("#hidVentaId").val() as string;
        var data = await self.api.get<model>(`${self.url}?ventaId=${ventaId}`);
        
        self.venta(data.venta);
        for (let it of data.devueltos) {
            self.lines.push(new ProductLine(it));
        }
    }


    public bind(): void {
        const self = this;
        BinderService.bind(self, "#devolucionesPage");
    }

    public async guardar(): Promise<void> {
        const self = this;
        alert("Falta funcionalidad para guardar");
    }
}

document.addEventListener('DOMContentLoaded', async function () {
    var page = new Devolucion();
    page.bind();
    console.log("binding ko");
    await page.load();
}, false);