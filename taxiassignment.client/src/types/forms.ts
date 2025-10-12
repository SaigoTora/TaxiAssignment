export type City = 'Kyiv' | 'Kharkiv' | 'Lviv'

export type GenerateData = {
	city: City
	taxiDriversCount?: number
	clientCount?: number
}

export type GenerateDataFormProps = {
	onGenerate: (data: GenerateData) => void
}

export type AssignData = {
	distances: number[][]
}
