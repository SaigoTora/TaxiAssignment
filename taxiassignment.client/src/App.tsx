import GenerateDataForm from './components/GenerateDataForm'
import { generateData } from './services/assignment'
import type { GenerateData } from './types/forms'

export default function App() {
	const onGenerate = async (inputData: GenerateData) => {
		await generateData(inputData)
	}

	return (
		<section className='flex flex-row items-start justify-start gap-12 p-8'>
			<div className='flex w-1/3 flex-col gap-10'>
				<GenerateDataForm onGenerate={inputData => onGenerate(inputData)} />
			</div>
		</section>
	)
}
