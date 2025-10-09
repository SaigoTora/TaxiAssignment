import { useEffect } from 'react'

export default function MapMarkers({ map, taxiDrivers, clients }: any) {
	useEffect(() => {
		if (!map) return

		const allInfoWindows: google.maps.InfoWindow[] = []

		const taxiMarkers = taxiDrivers.map((t: any) => {
			const marker = new google.maps.Marker({
				position: t,
				map,
				title: 'Taxi',
				icon: {
					url: 'https://img.icons8.com/?size=100&id=j2SfqDojO0h4&format=png&color=000000',
					scaledSize: new google.maps.Size(30, 30),
				},
				animation: google.maps.Animation.DROP,
			})

			const infoWindow = new google.maps.InfoWindow({
				content: `<div class='text-black'><b>Taxi Driver ID:</b> ${t.id}</div>`,
			})

			allInfoWindows.push(infoWindow)

			marker.addListener('click', () => {
				allInfoWindows.forEach(iw => iw.close())
				infoWindow.open(map, marker)
			})

			return marker
		})

		const clientMarkers = clients.map((c: any) => {
			const marker = new google.maps.Marker({
				position: c,
				map,
				title: 'Client',
				icon: {
					url: 'https://img.icons8.com/?size=100&id=9q3GMpxNIMjC&format=png&color=000000',
					scaledSize: new google.maps.Size(30, 30),
				},
				animation: google.maps.Animation.DROP,
			})

			const infoWindow = new google.maps.InfoWindow({
				content: `<div class='text-black'><b>Client ID:</b> ${c.id}</div>`,
			})

			allInfoWindows.push(infoWindow)

			marker.addListener('click', () => {
				allInfoWindows.forEach(iw => iw.close())
				infoWindow.open(map, marker)
			})

			return marker
		})

		const mapClickListener = map.addListener('click', () => {
			allInfoWindows.forEach(iw => iw.close())
		})

		return () => {
			taxiMarkers.forEach((m: { setMap: (arg0: null) => any }) =>
				m.setMap(null)
			)
			clientMarkers.forEach((m: { setMap: (arg0: null) => any }) =>
				m.setMap(null)
			)
			mapClickListener.remove()
		}
	}, [map, taxiDrivers, clients])

	return null
}
