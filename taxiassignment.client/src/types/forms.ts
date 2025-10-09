export type GenerateData = {
	taxiDriversCount?: number
	clientCount?: number
}

export type GenerateDataFormProps = {
	onGenerate: (data: GenerateData) => void
}
