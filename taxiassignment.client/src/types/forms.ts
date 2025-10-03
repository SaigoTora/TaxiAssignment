export type GenerateData = {
	taxiCount?: number
	clientCount?: number
}

export type GenerateDataFormProps = {
	onGenerate: (data: GenerateData) => void
}
