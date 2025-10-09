import { Box } from '@chakra-ui/react'
import { GoogleMap, useJsApiLoader } from '@react-google-maps/api'
import GenerateDataForm from './components/GenerateDataForm'
import MapMarkers from './components/MapMarkers'
import { generateData } from './services/assignment'
import type { GenerateData } from './types/forms'
import { useState } from 'react'

const GOOGLE_MAPS_API_KEY = import.meta.env.VITE_GOOGLE_MAPS_API_KEY

export default function App() {
	const [taxis, setTaxis] = useState<{ lat: number; lng: number }[]>([])
	const [clients, setClients] = useState<{ lat: number; lng: number }[]>([])
	const [map, setMap] = useState<google.maps.Map | null>(null)

	const { isLoaded } = useJsApiLoader({
		googleMapsApiKey: GOOGLE_MAPS_API_KEY,
		libraries: ['marker'],
	})

	const onGenerate = async (inputData: GenerateData) => {
		const data = await generateData(inputData)
		if (!data) return

		setTaxis(
			data.taxis.map((t: any) => ({
				lat: t.location.latitude,
				lng: t.location.longitude,
				id: t.id,
			}))
		)
		setClients(
			data.clients.map((c: any) => ({
				lat: c.location.latitude,
				lng: c.location.longitude,
				id: c.id,
			}))
		)
	}

	if (!isLoaded) return <div>Loading map...</div>

	return (
		<div className='relative w-full h-screen'>
			<GoogleMap
				mapContainerStyle={{ width: '100%', height: '100%' }}
				center={{ lat: 50.455, lng: 30.55 }}
				zoom={11}
				onLoad={mapInstance => setMap(mapInstance)}
			>
				{map && <MapMarkers map={map} taxis={taxis} clients={clients} />}
			</GoogleMap>

			<Box
				position='absolute'
				top='16px'
				left='16px'
				zIndex='10'
				bg='white'
				color='black'
				rounded='xl'
				shadow='md'
				p='4'
			>
				<GenerateDataForm onGenerate={onGenerate} />
			</Box>
		</div>
	)
}
