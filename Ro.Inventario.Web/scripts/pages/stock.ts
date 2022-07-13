import { BinderService } from '../services/binderService';
import { Api } from '../services/api';


export enum TipoAjuste
{
      Venta = 1,
      Merma,
      IngresoSinCompra
}
export interface StockAjusteModel
{
    productoId: string;
    tipoAjuste: TipoAjuste;
    cantidad: number;
    motivo: string;
}

export class Stock {

    public api: Api;
    public url: string;
    public stock: number;
    public operacion: KnockoutObservable<string>;
    public motivo: KnockoutObservable<string>;
    public cantidad: KnockoutObservable<number>;
    public stockFinal: KnockoutComputed<number>;

    constructor() {
        this.url = "productos/descargar";
        this.stock = $("#hidStock").val() as number;
        this.operacion = ko.observable<string>();
        this.motivo = ko.observable<string>();
        this.cantidad = ko.observable<number>();
        this.api = new Api();
        const self = this;
        this.stockFinal = ko.computed<number>(function () {
            switch (self.operacion()) {
                case "agregar":
                    return parseFloat(self.stock.toString()) + Math.abs(self.cantidad());
                case "quitar":
                    return parseFloat(self.stock.toString()) - Math.abs(self.cantidad());
                default: return 0;
            }
        });
    }
    public bind(): void {
        const self = this;
        BinderService.bind(self, "#stockPage");
    }

    public async guardar(): Promise<void> {
        const self = this;
        
        let data: StockAjusteModel = {
            productoId: $("#hidProductoId").val() as string,
            cantidad: Math.abs(self.cantidad()),
            motivo: self.motivo(),
            tipoAjuste: self.operacion() === "agregar" ? 
                        TipoAjuste.IngresoSinCompra : 
                        TipoAjuste.Merma
        };        

        await self.api.post(`Productos/Stock`, data);        

        alert("¡Guardado!");
        window.location.href = `${document.baseURI}Productos`;
    }
}

document.addEventListener('DOMContentLoaded', function () {
    var page = new Stock();
    page.bind();
    console.log("binding ko");
}, false);