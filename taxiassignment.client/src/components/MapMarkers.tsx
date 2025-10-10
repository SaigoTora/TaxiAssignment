import { useEffect } from 'react'

export default function MapMarkers({ map, taxiDrivers, clients }: any) {
	useEffect(() => {
		if (!map) return

		const allInfoWindows: google.maps.InfoWindow[] = []

		const taxiMarkers = taxiDrivers.map((t: any) => {
			const marker = new google.maps.Marker({
				position: t.location,
				map,
				title: 'Taxi',
				icon: {
					url: 'https://img.icons8.com/?size=100&id=j2SfqDojO0h4&format=png&color=000000',
					scaledSize: new google.maps.Size(30, 30),
				},
				animation:
					taxiDrivers.length + clients.length <= 500
						? google.maps.Animation.DROP
						: null,
			})

			const infoWindow = new google.maps.InfoWindow({
				content: `
				<div class="bg-white pt-4 pr-2 pb-1 pl-2 rounded-xl shadow-md max-w-xs font-sans text-gray-800">
					<div class="font-bold text-center text-base mb-1 cursor-text">
						${t.name} ${t.surname}, ${t.age}
					</div>
					<div class="text-gray-800 mb-3 cursor-text">
						${t.phoneNumber}
					</div>


					<div class="bg-gray-100 rounded-lg text-sm">
						<div class="flex items-center justify-between mb-1.5">
							<span class="font-semibold mr-2 cursor-text">${t.car.brand}</span>
							<span class="text-xs px-2 py-0.5 rounded-full bg-yellow-100 text-yellow-800 font-medium select-none">
								${t.car.class}
							</span>
						</div>

						<div class="flex items-center justify-between mb-1.5">
							<span class="flex items-center gap-1 mr-2">
								<span class="inline-block w-3 h-3 rounded-full border-2" style="background-color: ${t.car.color.toLowerCase()};"></span>
								<span class="cursor-text">${t.car.color}</span>
							</span>

							<span class="flex items-center font-mono text-xs bg-white border border-gray-300 rounded-md px-1.5 py-0.5 shadow-sm select-none">
								<span class="text-blue-600 mr-1 text-[10px]">
									🇺🇦
								</span>
								<span class="tracking-wider font-semibold">
									${t.car.licensePlate.replace(
										/^(\w{2})(\d{4})(\w{2})$/,
										'$1<span class="mx-[1px] inline-block"></span>$2<span class="mx-[1px] inline-block"></span>$3'
									)}
								</span>
							</span>
						</div>


						<div class="text-xs text-gray-500 text-right cursor-text">
							${t.car.seatsCount} seats
						</div>
      		</div>
				</div>`,
			})

			infoWindow.addListener('domready', () => {
				// Hiding the close button
				const closeBtn = document.querySelector<HTMLButtonElement>(
					'.gm-ui-hover-effect'
				)
				if (closeBtn) closeBtn.style.display = 'none'

				// Hide the top container (gm-style-iw-ch)
				const header = document.querySelector<HTMLDivElement>('.gm-style-iw-ch')
				if (header) header.style.display = 'none'
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
				position: c.location,
				map,
				title: 'Client',
				icon: {
					url: 'https://img.icons8.com/?size=100&id=9q3GMpxNIMjC&format=png&color=000000',
					scaledSize: new google.maps.Size(30, 30),
				},
				animation:
					taxiDrivers.length + clients.length <= 500
						? google.maps.Animation.DROP
						: null,
			})

			const infoWindow = new google.maps.InfoWindow({
				content: `
				<div class="bg-white pt-4 pr-2 pb-1 pl-2 rounded-xl shadow-md max-w-xs font-sans text-gray-800">
					<div class="font-bold text-center text-base mb-1 cursor-text">
						${c.name} ${c.surname}, ${c.age}
					</div>
					<div class="text-gray-800 mb-1 cursor-text">
						${c.phoneNumber}
					</div>
				</div>`,
			})

			infoWindow.addListener('domready', () => {
				// Hiding the close button
				const closeBtn = document.querySelector<HTMLButtonElement>(
					'.gm-ui-hover-effect'
				)
				if (closeBtn) closeBtn.style.display = 'none'

				// Hide the top container (gm-style-iw-ch)
				const header = document.querySelector<HTMLDivElement>('.gm-style-iw-ch')
				if (header) header.style.display = 'none'
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
