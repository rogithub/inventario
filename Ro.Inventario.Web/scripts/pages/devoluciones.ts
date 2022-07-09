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

export interface IAjusteProducto {
    id: string;
    oroductoId: string;
    ajusteId: string;
    cantidad: number;
    notas: string;
    precioUnitario: number;
}

interface model {
    venta: IAjuste,
    items: IAjusteProducto[]
}

class ProductLine {
    producto: IAjusteProducto;
    cantidadDevuelta: KnockoutObservable<number>;
    constructor(prod: IAjusteProducto) {
        this.producto = prod;
        this.cantidadDevuelta = ko.observable<number>();
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
        for (let it of data.items) {
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