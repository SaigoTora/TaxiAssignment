export type City = 'Kyiv' | 'Kharkiv' | 'Lviv'

export type GenerateData = {
	city: City
	taxiDriversCount?: number
	clientCount?: number
}

export type GenerateDataFormProps = {
	onGenerate: (data: GenerateData) => void
	onChange?: (data: GenerateData) => void
	disabled?: boolean
}

export type AssignData = {
	distances: number[][]
}

export interface AssignDataWithEpsilon extends AssignData {
	epsilonPrecision: number
}
