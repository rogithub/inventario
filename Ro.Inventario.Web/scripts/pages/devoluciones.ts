import { BinderService } from '../services/binderService';
import { Api } from '../services/api';

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
    constructor(l: IProductLine) {
        this.producto = l.producto;
        this.cantidadEnBuenasCondiciones = ko.observable<number>();
        this.cantidadEnMalasCondiciones = ko.observable<number>();
        this.ajusteProductoId = l.ajusteProductoId;
        this.cantidad = l.cantidad;
        this.precioUnitario = l.precioUnitario;
        this.categoria = l.categoria;
        this.unidadMedida = l.unidadMedida;
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

        console.dir(data);
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