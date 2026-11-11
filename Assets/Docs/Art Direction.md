# ART DIRECTION — No, Please Clean the Room!
## Gimersia 2026 · "Cozy Toy Diorama"

> Dokumen ini adalah aturan produksi, bukan moodboard. Semua angka di sini boleh diubah lewat ADR, bukan lewat selera per-aset.
> Rujukan terkait: `GDD.md`, `PackagesDocumentation.md`, `Animation Documentation.md`.

---

## 0. Cara Pakai Dokumen

| Peran | Bagian yang wajib dibaca |
| :--- | :--- |
| 3D Environment (Faris) | 4, 5, 6, 7, 8, 9, 11, 12, 13, 16, 17, 18, 19 |
| 3D Character & Animator (Haidar) | 5, 6, 9, 10, 11, 17, 19 + seluruh `Animation Documentation.md` |
| UI/UX & 2D (Irfan, Nikkow) | 6, 9, 15, 16, 19 |
| VFX/Juice (Fito) | 6, 10, 13, 14, 16, 19 |
| Programmer (Abdi, Raffa, Rasyid) | 4, 8, 9, 10, 13, 16, 17, 21 |

Aturan keras (sama dengan `Agentrule.md`): **tidak ada aset Generative AI**, jangan ubah file `.meta`, jangan ubah scene/prefab dari sisi agent.

---

## 1. Vision, Pilar, dan Mood

**Satu kalimat:** rumah mainan yang hangat dan lucu, dilihat dari atas, di mana kekacauan terasa menggemaskan alih-alih menjijikkan.

**Empat pilar (dipakai untuk memutuskan setiap aset):**

1. **Readability first.** Kamera jauh dan bisa split-screen. Objek harus dikenali dari *silhouette + warna* dalam < 0,5 detik, bukan dari detail kecil.
2. **Hangat dan jenaka.** Kekacauan itu komedi. Tidak ada kotoran realistis, darah, jamur seram, atau sudut gelap menakutkan.
3. **State = warna.** Kotor dan bersih dibedakan oleh saturasi, temperature, dan specular — bukan oleh teks atau ikon.
4. **Matte clay toy.** Permukaan matte, bentuk chunky, bevel lembut, tanpa kilap plastik.

**Mood keywords:** warm, bubbly, chunky, tidy-chaos, soft, playful, domestic, readable.
**Yang harus dihindari:** realistic PBR, glossy/plastik murah, outline hitam tebal gaya anime, tekstur noise mikro, pencahayaan dramatis berkontras tinggi, horror grime.

---

## 2. Referensi dan Anti-Referensi

**Referensi utama**
- *Moving Out 2* — siluet perabot chunky, dinding rendah, komedi fisika.
- *Overcooked! 2* — color-coding ruangan, HUD bersih, kamera 3/4 top-down.
- *Unpacking* — material matte, palet tenang, penataan yang memuaskan.
- *House Flipper* — kosakata aset rumah (bukan gayanya).

**Anti-referensi (jangan ditiru)**
- Render PBR realistis / arsitektural.
- Tekstur foto (fotogrametri) atau noise berlebihan.
- Kilap kaca/plastik tinggi, metal reflektif besar.
- Outline hitam pekat di semua objek.
- Detail yang hanya terlihat saat zoom dekat (sekrup, label kecil, kabel tipis).
- Grime coklat-hitam realistis; gunakan grime kartun yang lebih terang dan jenaka.

---

## 3. Kamera dan Aturan Readability

### 3.1 Tiga mode kamera

| Mode | Gaya | Kapan dipakai | Pitch / Yaw / FOV | Jarak | Catatan art |
| :--- | :--- | :--- | :--- | :--- | :--- |
| **Overview** | Moving Out | Awal level, transisi, layar hasil | 55° / 45° / 30° | 18–24 m | Semua ruangan harus terbaca; landmark tiap ruangan wajib terlihat |
| **Shared** | Overcooked 2 | Default co-op saat pemain berdekatan | 52° / 45° / 30° | 10–14 m | Fokus ke midpoint pemain; zoom mengikuti jarak pemain |
| **Split** | Split-screen | Saat pemain berjauhan | 50° / 45° / 30° | 7–9 m | Default **vertikal** (960×1080 per viewport); horizontal 1920×540 sebagai fallback |

- **Perspective**, bukan orthographic: FOV 30° memberi depth cue tanpa distorsi.
- Pitch dikunci per mode; **jangan** memutar yaw kamera saat gameplay.
- Transisi antar mode: 0,3–0,5 detik, tidak pernah snap.
- Yaw 45° membuat ruangan tampak diamond. Karena itu: **semua teks/label pada prop harus menghadap kamera** (rotate decal/UV), atau dihilangkan.

### 3.2 Aturan readability (uji sebelum aset dianggap selesai)

1. **Ukuran detail minimum:** bagian terkecil yang punya makna gameplay harus ≥ 10 cm di dunia nyata; proyeksi di layar ≥ 12 px pada 1080p di zoom terjauh.
2. **Kontras nilai:** objek interaktif harus berbeda **≥ 2 langkah nilai** dari lantai di belakangnya.
3. **Uji grayscale:** screenshot gameplay di-desaturasi. Kalau prop hilang menyatu dengan lantai, perbaiki nilai — bukan menambah outline.
4. **Uji split-screen:** setengah resolusi, dua viewport. Kalau objek tidak terbaca, perbesar/chunky-kan.
5. **Uji 0,5 detik:** tunjukkan frame ke orang lain selama 0,5 detik; mereka harus bisa menyebut objek utama.
6. **Lane bersih:** jalur gerak pemain minimal **1,2 m** lebar, tidak dipenuhi clutter.

### 3.3 Konsekuensi desain level

- Dinding rendah 1,0–1,5 m (lihat §12) agar isi ruangan selalu terlihat.
- Furnitur tinggi hanya menempel dinding belakang; **tidak ada objek > 1,8 m** di area tengah.
- Koridor minimal 1,2 m; hindari lorong panjang tanpa landmark.
- Setiap ruangan punya **satu landmark** dengan siluet unik (lihat §7).

---

## 4. Form Language dan Skala Mainan

**Aturan bentuk**
- Primitive dasar: *rounded box* dan *rounded cylinder*. Tidak ada bentuk bersudut tajam sebagai elemen utama.
- Bevel/chamfer: **2–4 cm** di semua tepi yang terlihat.
- Tidak ada elemen tipis < 6 cm (kaki meja, gagang, kabel). Perbesar sampai chunky.
- Handle, kenop, dan pegangan **dibesar-besarkan 1,3–1,5×** dari ukuran realistis agar terbaca.
- Detail dekoratif maksimal 2 tingkat (misal: garis panel + kenop), bukan 5.
- Hindari bentuk simetris sempurna; beri sedikit kemiringan/lekuk agar terasa hand-made.

**Skala mainan (penting)**
Dunia dibuat ±60% dari ukuran nyata agar karakter chibi terasa proporsional dan kamera terasa seperti melihat *dollhouse*.

| Elemen | Ukuran target |
| :--- | :--- |
| Tinggi karakter | 1,00–1,10 m |
| Pintu (lebar × tinggi) | 1,0 × 1,8 m |
| Tinggi dinding permainan | 1,2 m (±0,2) |
| Tebal dinding | 0,15 m |
| Meja makan | 0,55 m |
| Kursi (seat height) | 0,28 m |
| Sofa (seat height) | 0,30 m |
| Kulkas | 1,25 m |
| Kardus sedang | 0,35 × 0,35 × 0,35 m |
| Grid penempatan | 0,5 m |

**Aturan silhouette**
- Setiap prop harus punya "bentuk pembeda" dari jarak jauh (mis. kulkas = kotak tinggi aqua, sofa = blok rendah coral).
- Jangan mengandalkan tekstur untuk membedakan dua prop; bedakan bentuk dan nilai.

---

## 5. Palet Warna

### 5.1 Palet brand (terkunci dari GDD)

| Nama | Hex | Peran |
| :--- | :--- | :--- |
| Rose Salmon | `#F2AAAA` | Aksen lembut, kain, blush |
| Coral Blush | `#E38382` | Identitas P1, alert, CTA |
| Pastel Aqua | `#A8D8DF` | Identitas P2, kebersihan, air |
| Deep Plum | `#251F44` | **"Tinta"**: outline, tint shadow, teks, UI |

### 5.2 Palet tambahan (aditif, tidak mengganti brand)

| Nama | Hex | Peran |
| :--- | :--- | :--- |
| Paper Cream | `#FFF9F2` | Panel UI, dinding terang, highlight |
| Warm Wall | `#F3E7DA` | Dinding sekunder |
| Sand | `#E8D5C0` | Lantai keramik |
| Oak Light | `#D9B68F` | Lantai kayu terang |
| Oak Mid | `#C39A6B` | Furnitur kayu |
| Oak Dark | `#9C7350` | Aksen kayu / bayangan |
| Terracotta | `#C97B5A` | Pot, aksen hangat |
| Sage | `#A9C1A4` | Taman, tanaman terang |
| Leaf Deep | `#6E8B6A` | Tanaman gelap |
| Grime Light | `#B9A38A` | Kotoran level 1 |
| Grime Mid | `#8E7A5F` | Kotoran level 2 |
| Grime Heavy | `#5F5240` | Kotoran level 3 |
| Mold Green | `#6B7A5A` | Noda lembap kartun |
| Gold | `#F2C14E` | Koin / reward |
| Star Yellow | `#FFD166` | Bintang / sparkle |
| Critical Red | `#C94F4F` | Timer kritis |
| Skin P1 | `#E3B08A` | Kulit Suami |
| Skin P2 | `#F0C7A6` | Kulit Istri |
| Hair Dark | `#3A2E2A` | Rambut |
| Ink Soft | `#4A4368` | Teks sekunder, shadow lembut |

### 5.3 Aturan pemakaian

- **60 / 30 / 10:** 60% netral hangat (cream, sand, oak), 30% warna brand, 10% tinta plum.
- **Hitam murni dilarang.** Shadow, outline, teks memakai `#251F44` atau `#4A4368`.
- **Warna karakter dipesan.** Coral dan aqua *saturasi penuh* hanya untuk P1, P2, outline semantik, dan state UI. Lingkungan memakai versi *muted*: Aqua Muted `#CBE7EA`, Coral Muted `#EBA9A8`.
- **Prop interaktif:** jangan mewarnai seluruh bodi dengan coral/aqua. Beri panel/handle aksen kecil; outline yang memberi tahu "ini interaktif".
- Aksen emas hanya untuk reward (koin, bintang, sparkle), tidak untuk dekorasi ruangan.
- Setiap material punya **maksimal 3 langkah nilai** (base, shadow, highlight). Jangan gradien halus berlebihan.

### 5.4 Uji nilai (value test)

| Elemen | Target nilai (0–100) |
| :--- | :--- |
| Lantai | 70–82 |
| Dinding | 62–75 |
| Furnitur besar | 40–60 |
| Prop kecil / interaktif | 45–85 dengan aksen saturasi tinggi |
| Karakter | 50–75 + aksen cerah |
| Grime | 35–55 (lebih gelap dari permukaan bersih) |

---

## 6. Color Script per Ruangan

Tujuan: pemain tahu di ruangan mana dia **hanya dari warna**, penting untuk split-screen.

| Ruangan | Dominan | Aksen | Lantai | Landmark (siluet unik) |
| :--- | :--- | :--- | :--- | :--- |
| **Dojo (L0)** | Cream + Deep Plum | Coral, Gold | Tatami Sage/Sand | Torii mini + lampion |
| **Dapur (L1)** | Sand + Warm Wall | Coral Muted, Aqua Muted | Keramik Sand | Kulkas aqua tinggi |
| **Ruang TV (L1)** | Aqua Muted + Oak | Rose Salmon | Kayu Oak Light | Sofa coral rendah + TV |
| **Gudang (L1)** | Oak Dark + Cream | Rose Salmon | Beton Sand gelap | Tumpukan kardus tinggi |
| **Garden (L1)** | Sage + Cream | Aqua (langit) | Rumput Sage | Pohon besar + pagar |

Aturan: dua ruangan bersebelahan **tidak boleh** punya dominan yang sama. Landmark harus terlihat dari kamera Overview dan dari koridor.

---

## 7. Lighting (URP)

Prinsip: satu cahaya utama, lembut, hangat; sisanya ambient. Tidak ada GI realtime, tidak ada lampu dramatis.

**Directional Light**
- Rotation: X 52°, Y −35°, Z 0°.
- Color: `#FFE7D2` (hangat, ±4500 K).
- Intensity: 1,2 (URP).
- Shadows: Soft, strength 0,70, distance 30 m, cascade 2.

**Environment Lighting (gradient)**
- Sky: `#A8D8DF`, Equator: `#F3E7DA`, Ground: `#4A4368`.
- Intensity multiplier: 0,9.

**Ambient Occlusion**
- Prioritas: AO di vertex color / tekstur (murah, sesuai pipeline hybrid).
- Kekuatan AO: 0,15–0,25. Jangan sampai terlihat "kotor".
- Opsional (kalau FPS aman): SSAO Renderer Feature — Intensity 0,5, Radius 0,3, Samples Medium.

**Post-Processing (Volume global)**
- Tonemapping: Neutral, exposure 0.
- Bloom: threshold 1,0, intensity 0,15 — hanya untuk sparkle, emisi VFX, UI highlight.
- Color Adjustments: saturation +6, contrast +8.
- Vignette: **off**. Film grain: **off**. Chromatic aberration: **off**.
- Color space: Linear. HDR: on.

**Aturan**
- Tidak ada lampu titik/spot tambahan kecuali lampu dekoratif non-fungsional (tanpa shadow).
- Jangan bake pencahayaan dramatis ke tekstur; lighting berasal dari Unity.
- Shadow tidak boleh menutupi area gameplay hingga sulit membaca prop.

---

## 8. Material dan Tekstur (Krita)

**Pendekatan: hybrid.**
- Base color / flat material untuk mayoritas objek.
- Hand-paint hanya untuk detail yang menambah readability atau karakter: pola kain, serat kayu samar, pakaian karakter, wajah.
- **Tidak ada baked lighting atau shadow di tekstur.** Shading berasal dari URP + vertex AO.

**Spesifikasi tekstur**

| Jenis | Ukuran | Catatan |
| :--- | :--- | :--- |
| Atlas ruangan | 1024×1024 | Maksimal 1 atlas per ruangan |
| Atlas karakter | 1024×1024 per karakter | Boleh 512 untuk varian |
| Props kecil | berbagi atlas | Jangan buat tekstur per prop |
| UI/ikon | 128×128 source | Sprite atlas terpisah |
| VFX | 256×256 | Alpha, tanpa mipmap tajam |

**Aturan lukis (Krita)**
- Kanvas sRGB 8-bit, mulai dari palet dokumen ini; kunci palet.
- Maksimal 3 nilai per material; kuas flat/soft round, hindari tekstur noise.
- Line/garis tepi hanya jika membantu bentuk (mis. jahitan), warna `#4A4368`, bukan hitam.
- Ekspor PNG tanpa alpha kecuali butuh; jangan simpan layer berantakan.
- Satu set UV, texel density target: hero ±512 px/m, props ±256 px/m.

**Material URP/Lit**

| Permukaan | Smoothness | Metallic | Catatan |
| :--- | :--- | :--- | :--- |
| Kain / karpet | 0,05–0,15 | 0 | Matte penuh |
| Kayu | 0,20–0,30 | 0 | Sedikit sheen |
| Keramik / lantai bersih | 0,35–0,45 | 0 | Naik saat bersih |
| Dinding | 0,05–0,10 | 0 | Matte |
| Logam kecil (kenop, keran) | 0,45 | 0,6 | Hanya aksen kecil |
| Kotoran (grime) | 0,05 | 0 | Selalu matte, lebih gelap |

**Import settings tekstur**
- sRGB: on · Mipmaps: on · Aniso: 1 · Max size: 1024 · Compression: normal quality (BC7/DXT5 bila alpha).
- UI sprite: compression none/high quality, PPU 100, mesh type Full Rect.

---

## 9. Affordance dan Outline (Linework Lite)

Outline adalah **bahasa informasi**, bukan dekorasi. Gunakan `Free Outline` dengan rendering layer terpisah per makna.

| Makna | Warna | Lebar | Contoh |
| :--- | :--- | :--- | :--- |
| Interactable umum | `#A8D8DF` (aqua) | 1,5–2 px | Buku, piring, alat |
| Berat / butuh 2 pemain | `#E38382` (coral) | 2–2,5 px | Sofa, kulkas, lemari |
| Drop-zone valid | `#9FE3C2` (mint) | 1,5 px | Marker penataan |
| Sedang dipilih / hover | `#FFF9F2` (cream) | 2,5 px | Objek di tangan |
| Bahaya / timer kritis | `#C94F4F` | 2 px | Hanya UI, bukan objek |

**Aturan**
- Lebar outline konsisten di 1080p; jangan diskalakan per objek.
- Maksimal **satu** outline per objek; prioritaskan makna (berat > interactable).
- Outline **tidak boleh** dipakai untuk menggantikan kontras nilai yang buruk.
- Verifikasi di `Window > Linework Lite > Compatibility` sebelum menandai aset selesai.

---

## 10. Karakter

**Proporsi**
- Chibi **2,5–3 kepala**. Tinggi ±1,05 m.
- Kepala besar (25–30% tinggi total), tangan besar (±1,5×), kaki pendek.
- Bahu P1 lebar, perut bulat; P2 lebih ramping dengan siluet rambut (ponytail) yang asimetris.

**Identitas warna**

| | P1 Suami | P2 Istri |
| :--- | :--- | :--- |
| Aksen utama | Coral `#E38382` | Aqua `#A8D8DF` |
| Base pakaian | Cream `#FFF9F2` | Cream `#FFF9F2` |
| Sekunder | Oak Dark `#9C7350` | Deep Plum `#251F44` |
| Rambut | `#3A2E2A` | `#3A2E2A` dengan highlight plum |
| Kulit | `#E3B08A` | `#F0C7A6` |

**Siluet dari atas**
- P1: bulat + lebar, kepala besar; P2: lebih ramping + ponytail menonjol. Dari kamera top-down pun harus langsung terbedakan.
- Jangan gunakan coral/aqua saturasi penuh di lingkungan sekitar karakter pada ruangan yang sama.

**Wajah**
- Mata titik + mulut sederhana. Ekspresi memakai **sprite emoticon bubble** (lihat Animation doc), bukan blendshape.
- Maksimal 3 state tekstur wajah: normal, senang, panik (opsional, Tier B).

**Budget**
- ≤ 3.000 tris, ≤ 30 bone, 1 material (+ face swap).
- Tangan mitten (tanpa bone jari) dengan satu bone thumb opsional.

---

## 11. Environment dan Modular Kit

**Kit modular**
- Lantai: tile 1×1 m dan 2×2 m.
- Dinding: segmen 1 m, 2 m, dan sudut; tinggi 1,2 m, tebal 0,15 m.
- Pintu: frame + daun pintu 1,0 m; pintu geser untuk Level 2.
- Semua pivot di **dasar-tengah** objek (origin di lantai).

**Aturan penempatan**
- Grid 0,5 m.
- Furnitur besar menempel dinding, bukan di tengah lane.
- Area drop-zone ditandai marker transparan 30–40% + denyut halus; jangan memakai warna lantai.
- Karpet/rug tidak boleh terlihat seperti penghalang; hindari tepi tebal.
- Kepadatan clutter: 1 hero prop + 3–5 props sekunder per ±4 m². Sisakan lane.

**Dinding rendah**
- Tinggi 1,2 m, sisi luar boleh polos; sisi dalam boleh diberi panel/wainscot sederhana.
- Tidak perlu shader cutaway. Objek di belakang dinding tidak boleh lebih tinggi dari 1,8 m.
- Bila nanti dibutuhkan, fade saat menghalangi adalah Tier B (butuh shader terpisah).

**Garden**
- Tanaman low-poly chunky, pagar rendah, langit gradient (bukan skybox HDRI).
- Warna sage/leaf; jangan memakai aqua saturasi penuh (milik P2).

---

## 12. State Kotor ↔ Bersih

Ini sistem visual terpenting di game.

**Tiga level grime**

| Level | Cakupan | Warna | Contoh |
| :--- | :--- | :--- | :--- |
| L1 Ringan | 20–35% | Grime Light `#B9A38A` | Debu tipis, remah |
| L2 Sedang | 45–70% | Grime Mid `#8E7A5F` | Noda tumpahan, jejak kaki |
| L3 Berat | 80–100% | Grime Heavy `#5F5240` + Mold Green | Genangan, tumpukan |

**Aturan visual**
- Kotor = saturasi turun ±15%, temperature hangat/coklat, roughness 1,0, tanpa specular.
- Bersih = saturasi naik ±10%, specular 0,35, sparkle singkat.
- Transisi kotor → bersih **bertahap** (fade alpha mask), bukan swap mendadak.
- Marker kebersihan HUD memakai gradasi Grime Mid → Aqua → Cream.

**Implementasi (rekomendasi jam)**
- Overlay mesh transparan per permukaan (lantai/meja/dinding) dengan 1 mask texture per tipe permukaan.
- Fade alpha mengikuti progress pembersihan; hilangkan overlay saat 0%.
- URP Decal Projector adalah Tier B (lebih mahal, lebih fleksibel).
- Grime selalu matte dan lebih gelap dari permukaan bersih; jangan sampai terlihat seperti bayangan.

---

## 13. VFX dan Juice

Semua VFX 2D sprite/billboard, rendah partikel, warna dari palet.

| Efek | Warna | Bentuk | Durasi | Catatan |
| :--- | :--- | :--- | :--- | :--- |
| Dust puff | `#C9BBAE` + core cream | Bulat lembut | 0,4–0,6 s | 8–12 partikel, alpha |
| Sparkle bersih | `#FFD166` / `#FFF9F2` | Bintang 4 sudut | 0,4 s | Additive, 5–8 partikel |
| Bubble/sabun | `#A8D8DF` | Bulat transparan | 0,8 s | Alpha 0,6, mengapung |
| Chaos impact | Dust + garis komik | — | 0,15 s | Screen shake 0,1; Tier B |
| Emote bubble | Cream + ikon | Balon ucapan | 1,2 s | Pop via DOTween |
| Heart (Peluk) | `#E38382` | Hati kartun | 0,8 s | Muncul di antara dua karakter |
| Sweat drop (Bete) | `#A8D8DF` | Tetes keringat | 0,6 s | Loop halus saat mood rendah |

**Aturan**
- Maksimal ±300 partikel aktif total per viewport.
- Tidak ada efek blur berat, motion blur, atau lens flare.
- Warna VFX tidak boleh menutupi warna semantik outline.
- Semua efek dibuat dari sprite yang digambar manual (bukan AI).

---

## 14. UI dan Icon

**Kanvas**
- Referensi 1920×1080. CanvasScaler: Scale With Screen Size, match 0,5.
- Split-screen vertikal: tiap viewport 960×1080; HUD di-anchor dengan margin aman 64 px.
- Split horizontal (fallback): tiap viewport 1920×540; elemen HUD dipadatkan, timer tetap terlihat.

**Tipografi (OFL, gratis komersial)**
- Heading/logo: **Fredoka** (SemiBold/Bold).
- Body/HUD: **Nunito** atau **Baloo 2**.
- Ukuran: timer 64–72, label 28–32, body 22–24, minimum 18.
- Warna teks: `#251F44` di atas cream, cream di atas plum.

**Gaya visual**
- Panel cream `#FFF9F2` opasitas 90%, sudut membulat 8–12 px, outline 2 px `#251F44`, shadow lembut 20%.
- Tombol: coral untuk aksi utama, aqua untuk sekunder, plum untuk netral.
- Ikon: flat 2-tone (base color + outline ink), 128×128 source, tampil 64 px, latar transparan.
- Ikon dirender lewat `Icon Studio` dengan **kamera dan lighting yang sama** untuk semua ikon agar konsisten.

**HUD minimum**
- Timer (jam dinding) di atas-tengah atau kanan-atas.
- Cleanliness meter di tengah-atas; gradasi grime → aqua → cream.
- Mood meter kecil per pemain + emote wheel 4 pilihan (lihat GDD §15).
- Coin/star hanya di layar hasil.
- Tidak ada HUD yang menutupi lane tengah.

---

## 15. Technical Budget dan Render Settings

**Render (URP 17, Unity 6)**
- HDR on, MSAA 4×, SRP Batcher on.
- Shadow distance 30 m, cascade 2, soft shadows.
- Additional lights: off/vertex (tidak ada lampu tambahan fungsional).
- Post-processing volume global (lihat §7).

**Budget per viewport (split = render dua kali)**
- Triangle: ≤ 250k per viewport, ≤ 500k total scene.
- Draw call: ≤ 300 per viewport.
- Unique material: ≤ 60 di seluruh game.
- Texture memory: ≤ 64 MB.
- Target: 60 FPS di PC mid-range @1920×1080.
- Partikel aktif: ≤ 300 per viewport.

**Poly budget**

| Aset | Tris |
| :--- | :--- |
| Prop kecil | 50–300 |
| Prop sedang | 300–800 |
| Furnitur hero | ≤ 1.500 |
| Segmen dinding | 100–200 |
| Tile lantai | 2–12 |
| Karakter | ≤ 3.000 |

---

## 16. Pipeline Blender → Unity

**Blender**
1. Unit metric, scale 1,0; 1 unit = 1 m.
2. Apply transform (scale/rotation) sebelum export.
3. Pivot di dasar-tengah objek (origin di lantai).
4. Triangulate, hapus ngon, apply modifier.
5. Hapus kamera, lampu, dan objek tersembunyi.
6. Satu set UV; tidak ada baked lighting di tekstur.
7. Export FBX 7.4: Forward −Z, Up Y, Apply Scalings All Local, no animation untuk prop statis.

**Unity import mesh**
- Read/Write off, Mesh Compression off (aset kecil), Optimize on.
- Normals: import/calculate; tangents hanya jika ada normal map.
- Collider: box/convex sederhana; jangan pakai mesh collider untuk props kecil.

**Krita**
- Canvas 1024 (atau 512 untuk props), sRGB.
- Tanpa baked shadow; hanya albedo + detail.
- Ekspor PNG ke `Assets/Art/Textures/`.

---

## 17. Naming dan Struktur Folder

**Prefix**

| Prefix | Arti |
| :--- | :--- |
| `SM_` | Static mesh (prop, environment) |
| `SK_` | Skeletal mesh (karakter) |
| `T_` | Texture |
| `M_` | Material |
| `P_` | Prefab |
| `VFX_` | Efek partikel/sprite |
| `UI_` | Elemen UI |
| `ANIM_` | Animation clip (lihat Animation doc) |

Contoh: `SM_Kulkas_A`, `T_Dapur_Atlas`, `M_Kayu_Oak`, `P_Kardus_Sedang`.

**Struktur folder**

```text
Assets/Art/
├── Characters/
├── Environment/
│   ├── Modular/        (lantai, dinding, pintu)
│   └── Rooms/          (Dojo, Dapur, TV, Gudang, Garden)
├── Props/
│   ├── Kitchen/
│   ├── LivingRoom/
│   ├── Storage/
│   └── Garden/
├── UI/
│   ├── Icons/
│   ├── HUD/
│   └── Fonts/
├── VFX/
├── Materials/
├── Textures/
└── Animations/
```

Jangan ubah/ hapus file `.meta`. Jangan menyimpan aset di luar struktur ini tanpa alasan.

---

## 18. Checklist Review Aset

Sebuah aset **selesai** hanya jika semua lolos:

- [ ] Siluet terbaca di zoom terjauh dan di split-screen setengah resolusi.
- [ ] Skala sesuai tabel §4; pivot di dasar-tengah.
- [ ] Tri count dan jumlah material sesuai budget §15.
- [ ] Maksimal 3 nilai warna; tidak ada hitam murni.
- [ ] Tekstur memakai palet; tanpa baked lighting; setting import benar.
- [ ] Bevel 2–4 cm; tidak ada detail tipis < 6 cm.
- [ ] Material URP/Lit dengan smoothness/metallic sesuai §8.
- [ ] Rendering layer outline benar (interactable/berat/drop-zone).
- [ ] Collider sederhana dan pas.
- [ ] Nama file dan folder sesuai §17.
- [ ] Uji grayscale lulus (§3.2).
- [ ] Tidak melanggar aturan "no AI assets" dan tidak mengubah `.meta`.

---

## 19. Tier Prioritas dan Daftar Aset Sprint

**Tier A — wajib untuk vertical slice (Hari 1–5)**
- 2 karakter (mesh + rig + material).
- Modular kit: lantai, dinding 1,2 m, pintu, sudut.
- Palet 5 ruangan (Dojo, Dapur, TV, Gudang, Garden).
- Props inti: meja, kursi, sofa, kulkas, lemari, kasur, TV, rak, kardus (3 ukuran), trash bag, piring, buku, bantal.
- Alat: sapu, pel, kemoceng, ember.
- Grime overlay 3 level (lantai, meja, dinding).
- Drop-zone marker.
- 5 landmark ruangan.
- 20+ ikon UI inti (termasuk 4 ikon mood dan 4 ikon emote).
- Outline settings untuk 4 makna (§9).

**Tier B — polish (Hari 6–7)**
- Props dekoratif tambahan per ruangan.
- Varian karakter (aksen warna/ekspresi).
- Detail garden (tanaman, pagar, batu).
- Efek komik impact + screen shake.
- Art layar hasil (bintang, koin).
- Fade dinding (jika diperlukan).

**Tier C — pasca-jam**
- Decal Projector untuk grime.
- LOD, varian material, animasi wajah blendshape.

**Estimasi Level 0 (Dojo)**
| Chamber | Aset minimum |
| :--- | :--- |
| 0 Movement | Lantai tatami, dinding, torii, lampion |
| 1 Grab & Drop | 3 prop ringan, 1 drop-zone |
| 2 Cleaning | 1 permukaan kotor, sapu/pel, ember |
| 3 Throw | 1 platform + jurang, 2 prop lempar |
| 4 Two-player lift | 1 furnitur berat, 1 landmark |

**Estimasi Level 1 (Lebaran)**
| Ruangan | Aset minimum |
| :--- | :--- |
| Dapur | Kulkas, kompor, meja, piring, tumpahan |
| Ruang TV | Sofa, TV, karpet, bantal, debu |
| Gudang | 5 kardus, rak, karung |
| Garden | Pohon, pagar, pot, rumput |

---

## 20. Asumsi dan Pertanyaan Terbuka

1. Split-screen default **vertikal** (960×1080); horizontal sebagai fallback. Perlu dikonfirmasi setelah uji kamera.
2. Kamera **perspective FOV 30°**; orthographic hanya jika readability gagal.
3. Bahasa UI in-game: **Indonesia**.
4. Font memakai **Google Fonts OFL** (Fredoka, Nunito/Baloo 2).
5. Folder art baru: `Assets/Art/`.
6. Grime memakai overlay mesh (bukan Decal Projector) untuk performa.
7. Linework Lite perlu diverifikasi mendukung setting per-rendering-layer; jika tidak, pakai 1 settings dengan layer mask gabungan.

---

## 21. Catatan untuk Programmer

- Jangan mengubah material/texture saat runtime kecuali untuk state grime dan highlight.
- Gunakan `MaterialPropertyBlock` untuk variasi warna instan (hindari duplikasi material).
- Outline diatur lewat rendering layer, bukan menambah komponen per objek.
- Semua referensi visual (material, sprite, font) harus bisa di-swap via `[SerializeField]`.
- Budget 60 FPS diuji di build, bukan hanya di Editor.

---

*Dokumen ini hidup. Perubahan besar (kamera, tekstur, dinding) harus lewat ADR di `Assets/Docs/templates/adr.md`.*
