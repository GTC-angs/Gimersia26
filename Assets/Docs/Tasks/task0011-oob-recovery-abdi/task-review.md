# Review — TASK-0011

## Result

PASS WITH RISKS

## What changed

- Membuat script baru `Assets/Resources/Scripts/System/ItemOutOfBoundsRecoverer.cs`:
  - **SYS-09 (Out-of-Bounds Detection):** Deteksi ganda via Trigger Collider (`OnTriggerEnter`) dan ketinggian global (`position.y < _deathYThreshold`, default `-10f`).
  - **SYS-10 (Item Recovery & Cartoon Smoke VFX):**
    - Otomatis mencatat posisi dan rotasi awal setiap objek ber-Rigidbody di scene saat `Start()`.
    - Mereset `linearVelocity` dan `angularVelocity` ke `Vector3.zero` saat respawn.
    - Memindahkan objek kembali ke posisi spawn awal / titik tengah ruangan (`targetPos`).
    - Memunculkan partikel efek asap kartun (`_smokeVfxPrefab`) di titik respawn dan menghancurkan instance partikel setelah durasi selesai.
    - Menyediakan event `OnItemRecovered` dan API `RegisterItem` / `Recover` untuk integrasi eksternal.

## Acceptance criteria

- [x] Compile bersih (0 error).
- [ ] Deteksi OOB berfungsi saat item jatuh melewati `Y < -10f` atau masuk ke trigger zone OOB (menunggu smoke test Editor).
- [ ] Rigidbody linear & angular velocity dinolkan total saat respawn.
- [ ] Item kembali ke posisi spawn awal / titik tengah yang ditentukan.
- [ ] Efek asap kartun (`_smokeVfxPrefab`) muncul di titik respawn.
- [ ] Kompatibel dengan semua Rigidbody item interaktif.

## Regression check

- Related feature: Fisika perabot & drop-zone.
- Result: Objek kinematic (misal sedang dipegang atau terkunci drop-zone) tidak diintervensi oleh height checker.

## Verification evidence

- Compile check: `dotnet build Assembly-CSharp.csproj --no-restore` ➔ 0 errors, 0 warnings.

## Known issues

- Memerlukan smoke test di Unity Editor dengan objek nyata.

## Follow-up tasks

None.
