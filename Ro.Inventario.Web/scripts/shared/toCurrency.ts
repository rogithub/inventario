
const intl = new Intl.NumberFormat('es-MX', {
    style: "currency",
    currency: "USD",
    currencyDisplay: "narrowSymbol"
});

export default (val: number): string => {
    return intl.format(val);
}