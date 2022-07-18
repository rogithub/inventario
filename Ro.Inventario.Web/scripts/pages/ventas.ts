import { BinderService } from '../services/binderService';
import { Api } from '../services/api';
import toCurrency from '../shared/toCurrency';
import beep from '../shared/beep';

export interface IProduct {
    nid: number;
    id: string;
    nombre: string;
    unidadMedida: string;
    categoria: string;
    codigoBarrasItem: string;
    codigoBarrasCaja: string;
    precioVenta: number;
}


export interface VentaProductoModel {
    productoId: string;
    cantidad: number;
    precioUnitario: number;
}

export interface VentaModel {
    fecha: string;
    pago: number;
    cambio: number;
    items: VentaProductoModel[];
}

export class ProductLine {
    public producto: IProduct;
    public cantidad: KnockoutObservable<number>;
    public total: KnockoutComputed<number>;
    public totalPesos: KnockoutComputed<string>;
    public precioVentaPesos: KnockoutComputed<string>;
    public aMoneda = toCurrency;

    constructor(product: IProduct) {
        this.producto = product;
        this.cantidad = ko.observable<number>(1);
        const self = this;
        this.total = ko.computed<number>(function () {
            return self.cantidad() * self.producto.precioVenta;
        });
        this.totalPesos = ko.computed<string>(function () {
            return self.aMoneda(self.total());
        });
        this.precioVentaPesos = ko.computed<string>(function () {
            return self.aMoneda(self.producto.precioVenta);
        });
    }
}

export class Venta {
    public lines: KnockoutObservableArray<ProductLine>;
    public api: Api;
    public url: string;
    public autocomplete: Element;
    public total: KnockoutComputed<number>;
    public totalPesos: KnockoutComputed<string>;
    public pagoCliente: KnockoutObservable<number>;
    public cambio: KnockoutComputed<number>;
    public cambioPesos: KnockoutComputed<string>;
    public dateStr: KnockoutComputed<string>;
    public isValid: KnockoutComputed<boolean>;
    public editandoFecha: KnockoutObservable<boolean>;
    public date: KnockoutObservable<string>;
    public time: KnockoutObservable<string>;
    public fecha: Date;
    public aMoneda = toCurrency;
    public showQrScan: KnockoutObservable<boolean>;        

    constructor() {
        this.fecha = new Date();
        this.url = "ventas/Guardar"
        this.api = new Api();
        this.lines = ko.observableArray<ProductLine>([]);
        this.autocomplete = document.querySelector("#autoComplete");
        this.pagoCliente = ko.observable<number>(0);
        this.editandoFecha = ko.observable<boolean>(false);
        let mes = this.fecha.getMonth() + 1;
        let dia = this.fecha.getDate();
        let mesStr = mes < 10 ? '0' + mes.toString() : mes.toString();
        let diaStr = dia < 10 ? '0' + dia.toString() : dia.toString();
        let horas = this.fecha.getHours();
        let minutos = this.fecha.getMinutes();
        let horasStr = horas < 10 ? '0' + horas.toString() : horas.toString();
        let minutosStr = minutos < 10 ? '0' + minutos.toString() : minutos.toString();
        this.date = ko.observable<string>(`${this.fecha.getFullYear()}-${mesStr}-${diaStr}`);
        this.time = ko.observable<string>(`${horasStr}:${minutosStr}`);
        this.showQrScan = ko.observable<boolean>();        
        const self = this;        
        
        self.autocomplete.addEventListener("selection", function (e: any) {
            let p = e.detail.selection.value as IProduct;
            self.lines.push(new ProductLine(p));
            $(self.autocomplete).val("");
            return false;
        });

        this.total = ko.computed<number>(function () {
            let lines = self.lines();
            var initialValue = 0;
            return lines.reduce((sum, prod) => sum + prod.total(), initialValue);
        });
        this.totalPesos = ko.computed<string>(function () {            
            return self.aMoneda(self.total());
        });
        this.cambio = ko.computed<number>(function () {
            return self.pagoCliente() - self.total();
        });
        this.cambioPesos = ko.computed<string>(function () {
            return self.aMoneda(self.cambio());
        });

        this.isValid = ko.computed<boolean>(function () {
            for (const l of self.lines()) {
                if (l.cantidad() === 0 || isNaN(l.cantidad())) return false;
            }

            return !(isNaN(self.total()) || self.total() === 0 ||
                isNaN(self.pagoCliente()) || self.pagoCliente() === 0 ||
                self.total() > self.pagoCliente());
        });

        this.dateStr = ko.computed<string>(function () {
            var options: Intl.DateTimeFormatOptions = { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' };
            return new Date(self.parseDate()).toLocaleDateString("es-MX", options);
        });
    }

    public parseDate(): string {
        const self = this;
        return `${self.date()}T${self.time()}`;
    }

    public borrar(line: ProductLine): void {
        const self = this;
        self.lines.remove(line);
    }

    public bind(): void {
        const self = this;
        BinderService.bind(self, "#ventasPage");
    }

    public onScanSuccess(decodedText: any, decodedResult: any): void {
        const self = this;        
        console.log(`buscando por código ${decodedText}`);  
        self.api.get<IProduct[]>(`busquedas/GetByQr?qr=${decodedText}`).then(data => {
            if (data.length > 1){
                console.error(`Multiple products found ${decodedText}`);                
                return;
            }

            if (data.length === 0){
                alert(`Producto no encontrado para el código ${decodedText}`);                
                return;
            }

            data.forEach(p => {
                beep();
                let found = self.lines().find(l=>l.producto.id === p.id);
                if (found !== undefined && found !== null)
                {
                    let cantidad = parseFloat(found.cantidad().toString()) + parseFloat("1");
                    found.cantidad(cantidad);                    
                    return;
                }

                self.lines.push(new ProductLine(p));
            });            
        });
    }

    public onScanFailure(error: any): void {
        //const self = this;
        console.dir(error);
    }

    public readQr(): void {
        const self = this;
        let curr = self.showQrScan();
        self.showQrScan(!curr);
    }

    public async guardar(): Promise<void> {
        const self = this;
        let lines = new Array<VentaProductoModel>();
        self.lines().forEach(l => {
            let line: VentaProductoModel =
            {
                cantidad: l.cantidad(),
                productoId: l.producto.id,
                precioUnitario: l.producto.precioVenta
            };
            lines.push(line);
        });
        let data: VentaModel = {
            fecha: self.parseDate(),
            cambio: self.cambio() as number,
            pago: self.pagoCliente() as number,
            items: lines
        };
        let url = `${self.url}`;

        let result = await self.api.post<number[]>(url, data);
        console.log(`Ventas guardadas ${result[0]} productos en esa venta ${result[1]}`);

        alert("¡Guardado!");
        window.location.href = `${document.baseURI}`;
    }
}

document.addEventListener('DOMContentLoaded', function () {
    let page = new Venta();
    page.bind();
    console.log("binding ko");
    
    $("#reader").on("onScanSuccess", (e, decodedText, decodedResult) => {        
        page.onScanSuccess(decodedText, decodedResult);
    });

}, false);