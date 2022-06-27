export class OptionsItem<T> {
	public value: T;
	public text: string;
	constructor(text: string, value: T) {
		this.value = value;
		this.text = text;
	}
}


export class OptionsCollection<T> {
	public options: Array<OptionsItem<T>>;

	constructor(options: Array<OptionsItem<T>>) {
		this.options = options;
	}

	add(item: OptionsItem<T>): void {
		this.options.push(item);
	}

	get(value: T): OptionsItem<T> {
		const self = this;
		let item = ko.utils.arrayFirst(self.options, it => it.value === value);
		return item;
	}

	getValue(text: string): T {
		const self = this;
		let item = ko.utils.arrayFirst(self.options, it => it.text === text);
		return item.value;
	}

	getText(value: T): string {
		const self = this;
		return self.get(value).text;
	}
}