import { ObjectLiteral } from '../shared/objectLiteral';


/** Helper class to make http ajax requests */
export class Api {

	public baseURL: string;

	constructor() {
		this.baseURL = `${document.baseURI}`;
	}

	private getAntiforgeryToken(): string {
		let aftoken = $("input:hidden[name='__RequestVerificationToken']").val();
		return aftoken as string;
	}

	public get<T>(url: string): JQuery.Promise<T> {
		const self = this;
		return $.ajax({
			url: `${self.baseURL}${url}`,
			type: "GET",
			dataType: "json",
			contentType: "application/json; charset=utf-8"
		}) as JQuery.Promise<any>;
	}

	public post<T>(url: string, jsonData: ObjectLiteral): JQuery.Promise<T> {
		const self = this;
		return $.ajax({
			url: `${self.baseURL}${url}`,
			type: "POST",
			data: JSON.stringify(jsonData),
			dataType: "json",
			contentType: "application/json; charset=utf-8",
			headers: {
				'RequestVerificationToken': self.getAntiforgeryToken()
			}
		}) as JQuery.Promise<any>;
	}

	public put<T>(url: string, jsonData: ObjectLiteral): JQuery.Promise<T> {
		const self = this;
		return $.ajax({
			url: `${self.baseURL}${url}`,
			type: "PUT",
			data: JSON.stringify(jsonData),
			dataType: "json",
			contentType: "application/json; charset=utf-8",
			headers: {
				'RequestVerificationToken': self.getAntiforgeryToken()
			}
		}) as JQuery.Promise<any>;
	}

	public patch<T>(url: string, jsonData: ObjectLiteral): JQuery.Promise<T> {
		const self = this;
		return $.ajax({
			url: `${self.baseURL}${url}`,
			type: "PATCH",
			data: JSON.stringify(jsonData),
			dataType: "json",
			contentType: "application/json; charset=utf-8",
			headers: {
				'RequestVerificationToken': self.getAntiforgeryToken()
			}
		}) as JQuery.Promise<any>;
	}

	public del<T>(url: string): JQuery.Promise<T> {
		const self = this;
		return $.ajax({
			url: `${self.baseURL}${url}`,
			type: "DELETE",
			dataType: "json",
			contentType: "application/json; charset=utf-8",
			headers: {
				'RequestVerificationToken': self.getAntiforgeryToken()
			}
		}) as JQuery.Promise<any>;
	}
}