import { BinderService } from '../services/binderService';
import { Api } from '../services/api';
import toCurrency from '../shared/toCurrency';

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
    devolucionRowOpPesos: KnockoutComputed<string>;
    precioUnitarioPesos: KnockoutComputed<string>;
    totalPesos: KnockoutComputed<string>;
    hasError: KnockoutComputed<boolean>;
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

        this.devolucionRowOpPesos = ko.computed<string>(() => {
            return toCurrency(self.devolucionRowOp());
        }, self);

        this.precioUnitarioPesos = ko.computed<string>(() => {
            return toCurrency(self.precioUnitario);
        }, self);
        this.totalPesos = ko.computed<string>(() => {
            return toCurrency((self.precioUnitario * l.cantidad));
        }, self);

        this.hasError = ko.computed<boolean>(() => {
            let a = this.cantidadEnBuenasCondiciones();
            let b = this.cantidadEnMalasCondiciones();  
            if (isNaN(a) || isNaN(b)) return true;

            let c = this.cantidad;            
            return (parseFloat(a.toString()) + parseFloat(b.toString())) > c;

        }, self);        

        this.aMoneda = toCurrency;
    }
}

export interface DevolucionProductoModel {
    ajusteProductoId: string;
    cantidadEnBuenasCondiciones: number;
    cantidadEnMalasCondiciones: number;
}

export class Devolucion {
    public api: Api;
    public url: string;
    public lines: KnockoutObservableArray<ProductLine>;
    public venta: KnockoutObservable<IAjuste>;
    public isValid: KnockoutComputed<boolean>;
    public totalDevolucion: KnockoutComputed<string>;;
    constructor() {
        this.url = "ventas/GetVentaData"
        this.api = new Api();
        this.lines = ko.observableArray<ProductLine>();
        this.venta = ko.observable<IAjuste>();
        const self = this;
        
        this.isValid = ko.computed<boolean>(() => {
            let count = 0;
            for(const it of self.lines())
            {
                if (it.hasError()) return false;
                count += it.cantidadEnBuenasCondiciones();
                count += it.cantidadEnMalasCondiciones();
            }
            return count > 0;
        }, self);       

        this.totalDevolucion = ko.computed<string>(() => {
            let suma = self.lines().reduce((acc, line, i)=>{
                return acc + line.devolucionRowOp();
            }, 0);

            return toCurrency(suma);

        }, self);       
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
        let data = [];
        for(const l of self.lines())
        {
            let item: DevolucionProductoModel = {
                ajusteProductoId: l.ajusteProductoId,
                cantidadEnBuenasCondiciones: l.cantidadEnBuenasCondiciones(),
                cantidadEnMalasCondiciones: l.cantidadEnMalasCondiciones(),            
            };        
            data.push(item);
        }                

        await self.api.post(`Ventas/DevolverProductos`, data);        

        alert("¡Guardado!");
        window.location.href = `${document.baseURI}Ventas`;
    }
}

document.addEventListener('DOMContentLoaded', async function () {
    var page = new Devolucion();
    page.bind();
    console.log("binding ko");
    await page.load();
}, false);