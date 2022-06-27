


export class RowSplitter {
	public static toRows<T>(list: T[], itemsPerRow: number): T[][] {
		let collector: T[][] = [];

		list.reduce((acc, feed, i, arr): T[][] => {
			if (acc.length === 0 || acc[acc.length - 1].length === itemsPerRow) {
				acc.push([]);
			}

			let current = acc[acc.length - 1];
			current.push(feed);

			return acc;
		}, collector);

		return collector;
	}
}