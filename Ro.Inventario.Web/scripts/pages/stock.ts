import { BinderService } from '../services/binderService';
import { Api } from '../services/api';


export class Stock {

    public api: Api;
    public url: string;    

    constructor() {        
        this.url = "productos/descargar"
        this.api = new Api();                    
    }    
    public bind(): void {
        const self = this;
        BinderService.bind(self, "#stockPage");
    }

    public guardar(): void {
        const self = this;
        alert("Todavia no guarda, estamos trabajando...");
    }
}

document.addEventListener('DOMContentLoaded', function () {
    var page = new Stock();
    page.bind();
    console.log("binding ko");
}, false);